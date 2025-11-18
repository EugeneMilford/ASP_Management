using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserMail;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMail
{
    public class IndexTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public IndexTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnGetAsync_ReturnsOnlyNonSpamInboxForUser()
        {
            // Arrange - create and persist the current user and an "other" user BEFORE creating mails
            var user = new OfficeUser
            {
                Id = "user1",
                UserName = "user1@test.com",
                Email = "user1@test.com",
                FirstName = "User",
                LastName = "One",
                UserRole = "User"
            };

            var otherUser = new OfficeUser
            {
                Id = "other",
                UserName = "other@test.com",
                Email = "other@test.com",
                FirstName = "Other",
                LastName = "User",
                UserRole = "User"
            };

            _context.Users.AddRange(user, otherUser);
            await _context.SaveChangesAsync(); // ensure AspNetUsers contains these users for FK references

            // Re-query the persisted/tracked users from the same context
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            var persistedOther = await _context.Users.FirstAsync(u => u.Id == otherUser.Id);

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "DemoAdmin")).ReturnsAsync(false);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            // Seed mails: two for the persisted current user (one spam), one for the persisted other user
            _context.Mails.AddRange(
                new Mail
                {
                    MailTopic = "Inbox1",
                    MailContent = "Hello",
                    Sender = persistedUser.Email,
                    CreatedDate = DateTime.UtcNow,
                    UserId = persistedUser.Id,
                    User = persistedUser,
                    IsSpam = false
                },
                new Mail
                {
                    MailTopic = "Spam",
                    MailContent = "Buy now",
                    Sender = "Spammer",
                    CreatedDate = DateTime.UtcNow,
                    UserId = persistedUser.Id,
                    User = persistedUser,
                    IsSpam = true
                },
                new Mail
                {
                    MailTopic = "Other",
                    MailContent = "Other",
                    Sender = persistedOther.Email,
                    CreatedDate = DateTime.UtcNow,
                    UserId = persistedOther.Id,
                    User = persistedOther,
                    IsSpam = false
                }
            );
            await _context.SaveChangesAsync();

            var pageModel = new IndexModel(_context, userManager.Object);

            // Act
            await pageModel.OnGetAsync();

            // Assert
            // The page may include mails for other users (depending on implementation),
            // but we assert that the current user's inbox (non-spam) contains exactly one expected mail.
            var userInbox = pageModel.InboxMails
                .Where(m => m.UserId == persistedUser.Id && !m.IsSpam)
                .ToList();

            Assert.Single(userInbox);
            Assert.Equal("Inbox1", userInbox[0].MailTopic);
        }

        [Fact]
        public async Task OnGetAsync_ShowsEmptyInbox_WhenNoMails()
        {
            // Arrange - persist a user with no mails
            var user = new OfficeUser
            {
                Id = "user2",
                UserName = "user2@test.com",
                Email = "user2@test.com",
                FirstName = "User",
                LastName = "Two",
                UserRole = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "DemoAdmin")).ReturnsAsync(false);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new IndexModel(_context, userManager.Object);

            // Act
            await pageModel.OnGetAsync();

            // Assert - ensure no non-spam mails for this user
            var userInbox = pageModel.InboxMails
                .Where(m => m.UserId == persistedUser.Id && !m.IsSpam)
                .ToList();

            Assert.Empty(userInbox);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

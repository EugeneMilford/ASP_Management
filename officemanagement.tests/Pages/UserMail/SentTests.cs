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
    public class SentTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public SentTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnGetAsync_ReturnsSentMails_ForCurrentUser()
        {
            // Arrange - create and persist the sending user BEFORE creating mails
            var user = new OfficeUser
            {
                Id = "sender1",
                UserName = "sender1@test.com",
                Email = "sender1@test.com",
                FirstName = "Sender",
                LastName = "One",
                UserRole = "User"
            };

            // Also create a different user to represent "other" mails (satisfy FK)
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

            // Re-query the persisted/tracked user
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            var persistedOther = await _context.Users.FirstAsync(u => u.Id == otherUser.Id);

            // Setup mocked UserManager to return the persisted user and Users IQueryable
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "DemoAdmin")).ReturnsAsync(false);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            // Seed mails: two sent by current user, one by another user (use persisted ids)
            _context.Mails.AddRange(
                new Mail
                {
                    MailTopic = "Sent1",
                    MailContent = "Hi",
                    Sender = persistedUser.Email,
                    CreatedDate = DateTime.UtcNow,
                    UserId = persistedUser.Id,
                    User = persistedUser,
                    IsSpam = false
                },
                new Mail
                {
                    MailTopic = "Sent2",
                    MailContent = "Hi again",
                    Sender = persistedUser.Email,
                    CreatedDate = DateTime.UtcNow,
                    UserId = persistedUser.Id,
                    User = persistedUser,
                    IsSpam = false
                },
                new Mail
                {
                    MailTopic = "OtherSent",
                    MailContent = "Other",
                    Sender = persistedOther.Email,
                    CreatedDate = DateTime.UtcNow,
                    UserId = persistedOther.Id,
                    User = persistedOther,
                    IsSpam = false
                }
            );
            await _context.SaveChangesAsync();

            var pageModel = new SentModel(_context, userManager.Object);

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.SentMails);
            var sent = pageModel.SentMails.ToList();
            Assert.Equal(2, sent.Count);
            Assert.All(sent, m => Assert.Equal(persistedUser.Id, m.UserId));
        }

        [Fact]
        public async Task OnGetAsync_SentEmpty_WhenUserHasNoSentMails()
        {
            // Arrange - persist a user with no sent mails
            var user = new OfficeUser
            {
                Id = "sender2",
                UserName = "sender2@test.com",
                Email = "sender2@test.com",
                FirstName = "Sender",
                LastName = "Two",
                UserRole = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "DemoAdmin")).ReturnsAsync(false);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new SentModel(_context, userManager.Object);

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.SentMails);
            Assert.Empty(pageModel.SentMails);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

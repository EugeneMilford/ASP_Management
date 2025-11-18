using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserMessages;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMessages
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
        public async Task OnGetAsync_ShowsOnlyMessagesAddressedToCurrentUser()
        {
            // Arrange: persist recipient (current user) and a sender and an unrelated user
            var recipient = new OfficeUser { Id = "recipient1", UserName = "r1@test.com", Email = "r1@test.com", FirstName = "Rec", LastName = "One", UserRole = "User" };
            var sender = new OfficeUser { Id = "sender1", UserName = "s1@test.com", Email = "s1@test.com", FirstName = "Send", LastName = "One", UserRole = "User" };
            var other = new OfficeUser { Id = "other", UserName = "other@test.com", Email = "other@test.com", FirstName = "Other", LastName = "User", UserRole = "User" };

            _context.Users.AddRange(recipient, sender, other);
            await _context.SaveChangesAsync();

            // Re-query tracked instances
            var persistedRecipient = await _context.Users.FirstAsync(u => u.Id == recipient.Id);
            var persistedSender = await _context.Users.FirstAsync(u => u.Id == sender.Id);
            var persistedOther = await _context.Users.FirstAsync(u => u.Id == other.Id);

            // Seed messages: two to recipient (different senders), one to other user
            _context.Messages.AddRange(
                new Message { FromUserId = persistedSender.Id, ToUserId = persistedRecipient.Id, Content = "Hello 1", SentDate = DateTime.UtcNow.AddMinutes(-10), FromUser = persistedSender, ToUser = persistedRecipient },
                new Message { FromUserId = persistedOther.Id, ToUserId = persistedRecipient.Id, Content = "Hello 2", SentDate = DateTime.UtcNow.AddMinutes(-5), FromUser = persistedOther, ToUser = persistedRecipient },
                new Message { FromUserId = persistedSender.Id, ToUserId = persistedOther.Id, Content = "Not for recipient", SentDate = DateTime.UtcNow.AddMinutes(-1), FromUser = persistedSender, ToUser = persistedOther }
            );
            await _context.SaveChangesAsync();

            // Mock UserManager to return the recipient as current user
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedRecipient);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new IndexModel(_context, userManager.Object);

            // Act
            await pageModel.OnGetAsync();

            // Assert: Messages collection should include only messages where ToUserId == recipient.Id
            Assert.NotNull(pageModel.Messages);
            var inbox = pageModel.Messages.Where(m => m.ToUserId == persistedRecipient.Id).ToList();
            Assert.Equal(2, inbox.Count);
            Assert.Contains(inbox, m => m.Content == "Hello 1");
            Assert.Contains(inbox, m => m.Content == "Hello 2");
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

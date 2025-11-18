using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserMessages;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMessages
{
    public class DeleteTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public DeleteTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnPostDeleteAsync_RemovesMessage_WhenIdValid()
        {
            // Arrange: create & persist sender and recipient
            var sender = new OfficeUser { Id = "sender1", UserName = "s1@test.com", Email = "s1@test.com", FirstName = "Send", LastName = "One", UserRole = "User" };
            var recipient = new OfficeUser { Id = "recipient1", UserName = "r1@test.com", Email = "r1@test.com", FirstName = "Rec", LastName = "One", UserRole = "User" };
            _context.Users.AddRange(sender, recipient);
            await _context.SaveChangesAsync();

            var persistedSender = await _context.Users.FirstAsync(u => u.Id == sender.Id);
            var persistedRecipient = await _context.Users.FirstAsync(u => u.Id == recipient.Id);

            // seed a message addressed to the recipient
            var msg = new Message
            {
                FromUserId = persistedSender.Id,
                ToUserId = persistedRecipient.Id,
                Content = "To be deleted",
                SentDate = DateTime.UtcNow,
                FromUser = persistedSender,
                ToUser = persistedRecipient
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Mock UserManager to return the recipient as the current user
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedRecipient);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new IndexModel(_context, userManager.Object);

            // Act
            var result = await pageModel.OnPostDeleteAsync(msg.MessageId);

            // Assert: handler redirects and message removed
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Null(await _context.Messages.FindAsync(msg.MessageId));
        }

        [Fact]
        public async Task OnPostDeleteAsync_ReturnsRedirect_WhenIdInvalid()
        {
            // Arrange: persist a recipient user so mock can return a current user
            var recipient = new OfficeUser { Id = "recipient2", UserName = "r2@test.com", Email = "r2@test.com", FirstName = "Rec", LastName = "Two", UserRole = "User" };
            _context.Users.Add(recipient);
            await _context.SaveChangesAsync();
            var persistedRecipient = await _context.Users.FirstAsync(u => u.Id == recipient.Id);

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedRecipient);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new IndexModel(_context, userManager.Object);

            // Act - call delete with an ID that doesn't exist
            var result = await pageModel.OnPostDeleteAsync(99999);

            // Assert - Page currently redirects even when the message doesn't exist
            Assert.IsType<RedirectToPageResult>(result);

            // no messages should have been removed (empty DB)
            Assert.Empty(await _context.Messages.ToListAsync());
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}
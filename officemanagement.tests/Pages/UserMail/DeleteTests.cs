using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserMail;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMail
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
        public async Task OnPostAsync_Delete_RemovesMail()
        {
            // Arrange - create and persist the user so the Mail.UserId FK is valid
            var user = new OfficeUser
            {
                Id = "alice",
                UserName = "alice@test.com",
                Email = "alice@test.com",
                FirstName = "Alice",
                LastName = "Example",
                UserRole = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // persist user into AspNetUsers

            // Re-query to get the tracked/persisted user instance from the same context
            var persisted = await _context.Users.FirstAsync(u => u.Id == user.Id);

            // seed a mail record referencing the existing persisted user
            var mail = new Mail
            {
                MailTopic = "ToDelete",
                MailContent = "Delete me",
                Sender = persisted.Email,
                CreatedDate = DateTime.UtcNow,
                UserId = persisted.Id,
                User = persisted
            };
            _context.Mails.Add(mail);
            await _context.SaveChangesAsync(); // now this should succeed because the FK target exists

            var deleteModel = new DeleteModel(_context);

            // Act
            var result = await deleteModel.OnPostAsync(mail.MailId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Null(await _context.Mails.FindAsync(mail.MailId));
        }

        [Fact]
        public async Task OnPostAsync_Delete_ReturnsNotFound_WhenIdInvalid()
        {
            var deleteModel = new DeleteModel(_context);

            var result = await deleteModel.OnPostAsync(9999);

            Assert.IsType<NotFoundResult>(result);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

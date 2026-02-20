using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserMail;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMail
{
    public class ViewTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public ViewTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnGetAsync_ReturnsMail_WhenIdIsValid()
        {
            // Arrange - create and persist the user referenced by the mail so FK is satisfied
            var user = new OfficeUser
            {
                Id = "bob",
                UserName = "bob@test.com",
                Email = "bob@test.com",
                FirstName = "Bob",
                LastName = "Tester",
                UserRole = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Re-query the persisted/tracked user
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var mail = new Mail
            {
                MailTopic = "ViewMe",
                MailContent = "Details",
                Sender = persistedUser.Email,
                CreatedDate = DateTime.UtcNow,
                UserId = persistedUser.Id,
                User = persistedUser
            };
            _context.Mails.Add(mail);
            await _context.SaveChangesAsync();

            var detailsModel = new DetailsModel(_context);

            // Act
            var result = await detailsModel.OnGetAsync(mail.MailId);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(detailsModel.Mail);
            Assert.Equal(mail.MailId, detailsModel.Mail.MailId);
            Assert.Equal("ViewMe", detailsModel.Mail.MailTopic);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsNotFound_WhenIdIsInvalid()
        {
            var detailsModel = new DetailsModel(_context);

            var result = await detailsModel.OnGetAsync(9999);

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

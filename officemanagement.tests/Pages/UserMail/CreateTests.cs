using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserMail;
using OfficeManagement.Tests.TestHelpers;
using Xunit;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfficeManagement.Tests.Pages.UserMail
{
    public class CreateTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public CreateTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnPostAsync_AddsNewMail_ForAuthenticatedUser()
        {
            // Arrange - create sender and recipient users
            var sender = new OfficeUser { Id = "user1", UserName = "user1@test.com", Email = "user1@test.com", FirstName = "User", LastName = "One", UserRole = "User" };
            var recipient = new OfficeUser { Id = "user2", UserName = "user2@test.com", Email = "user2@test.com", FirstName = "User", LastName = "Two", UserRole = "User" };
            _context.Users.AddRange(sender, recipient);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(sender);
            // Ensure UserManager.Users returns the IQueryable from the context for OnGetAsync
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Input = new CreateModel.MailInputModel
                {
                    MailTopic = "Hello",
                    MailContent = "This is a test mail",
                    UserId = recipient.Id
                }
            };

            // Act
            var result = await createModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var saved = await _context.Mails.FirstOrDefaultAsync(m => m.MailTopic == "Hello");
            Assert.NotNull(saved);
            Assert.Equal(sender.Email, saved.Sender);
            Assert.Equal(recipient.Id, saved.UserId); // recipient stored as UserId
            Assert.False(saved.IsSpam);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateInvalid()
        {
            // Arrange - create a user and ensure Users is available for OnGetAsync call
            var sender = new OfficeUser { Id = "user3", UserName = "user3@test.com", Email = "user3@test.com", FirstName = "User", LastName = "Three", UserRole = "User" };
            _context.Users.Add(sender);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(sender);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Input = new CreateModel.MailInputModel()
            };
            createModel.ModelState.AddModelError("Input.MailTopic", "Required");

            // Act
            var result = await createModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Empty(await _context.Mails.ToListAsync());
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}
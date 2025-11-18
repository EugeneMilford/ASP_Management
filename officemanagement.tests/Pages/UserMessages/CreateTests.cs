using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Pages.UserMessages;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMessages
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
        public async Task OnPostAsync_CreatesNewMessage_WithCurrentUserAsSender()
        {
            // Arrange: create and persist sender (current user) and recipient
            var sender = new OfficeUser
            {
                Id = "sender1",
                UserName = "s1@test.com",
                Email = "s1@test.com",
                FirstName = "Send",
                LastName = "One",
                UserRole = "User"
            };
            var recipient = new OfficeUser
            {
                Id = "recipient1",
                UserName = "r1@test.com",
                Email = "r1@test.com",
                FirstName = "Rec",
                LastName = "One",
                UserRole = "User"
            };
            _context.Users.AddRange(sender, recipient);
            await _context.SaveChangesAsync();

            // Re-query tracked entities from the same context
            var persistedSender = await _context.Users.FirstAsync(u => u.Id == sender.Id);
            var persistedRecipient = await _context.Users.FirstAsync(u => u.Id == recipient.Id);

            // Create/mock UserManager and make sure GetUserAsync returns the persisted sender for any principal
            var userManagerMock = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedSender);
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedSender);
            userManagerMock.Setup(m => m.Users).Returns(_context.Users);

            var createModel = new CreateModel(_context, userManagerMock.Object)
            {
                Input = new CreateModel.MessageInputModel
                {
                    ToUserId = persistedRecipient.Id,
                    Content = "Test message content"
                }
            };

            // Provide a ClaimsPrincipal on the PageContext so PageModel.User is non-null and matches what GetUserAsync receives
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedSender.Id),
                new Claim(ClaimTypes.Name, persistedSender.UserName)
            }, "TestAuth"));

            createModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            // Provide a TempData dictionary so TempData["Success"] access in the page doesn't throw NRE
            createModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await createModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);

            var saved = await _context.Messages.FirstOrDefaultAsync(m => m.Content == "Test message content");
            Assert.NotNull(saved);
            Assert.Equal(persistedSender.Id, saved.FromUserId);
            Assert.Equal(persistedRecipient.Id, saved.ToUserId);
            Assert.True((DateTime.UtcNow - saved.SentDate).TotalMinutes < 5);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateInvalid()
        {
            // Arrange
            var sender = new OfficeUser { Id = "sender2", UserName = "s2@test.com", Email = "s2@test.com", FirstName = "Send", LastName = "Two", UserRole = "User" };
            _context.Users.Add(sender);
            await _context.SaveChangesAsync();
            var persistedSender = await _context.Users.FirstAsync(u => u.Id == sender.Id);

            var userManagerMock = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedSender);
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedSender);
            userManagerMock.Setup(m => m.Users).Returns(_context.Users);

            var createModel = new CreateModel(_context, userManagerMock.Object)
            {
                Input = new CreateModel.MessageInputModel
                {
                    ToUserId = "", // invalid
                    Content = ""   // invalid
                }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedSender.Id),
                new Claim(ClaimTypes.Name, persistedSender.UserName)
            }, "TestAuth"));
            createModel.PageContext = new PageContext { HttpContext = httpContext };

            // Provide TempData here as well (OnPostAsync sets TempData on success path; adding it avoids potential NREs)
            createModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            createModel.ModelState.AddModelError("Input.ToUserId", "Required");

            // Act
            var result = await createModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
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
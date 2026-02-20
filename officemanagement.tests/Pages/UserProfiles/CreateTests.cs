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
using OfficeManagement.Models;
using OfficeManagement.Pages.UserProfiles;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserProfiles
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
        public async Task OnPostAsync_CreatesProfile_AssignsToCurrentUser()
        {
            // Arrange - create and persist a user
            var user = new OfficeUser { Id = "profileUser1", UserName = "p1@test.com", Email = "p1@test.com", FirstName = "P", LastName = "One", UserRole = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedUser);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Profile = new Profile
                {
                    ProfileName = "P",
                    ProfileSurname = "One",
                    Title = "Tester",
                    ProfileDescription = "Desc",
                    Experience = "X",
                    Education = "Y",
                    Skills = "Z",
                    Location = "Loc",
                    Hobbies = "None",
                    Notes = "Notes",
                    DateJoined = DateTime.UtcNow
                }
            };

            // Provide principal on PageContext and TempData
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedUser.Id),
                new Claim(ClaimTypes.Name, persistedUser.UserName)
            }, "TestAuth"));
            createModel.PageContext = new PageContext { HttpContext = httpContext };
            createModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await createModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var saved = await _context.Summary.FirstOrDefaultAsync(p => p.ProfileName == "P" && p.UserId == persistedUser.Id);
            Assert.NotNull(saved);
            Assert.Equal(persistedUser.Id, saved.UserId);
            Assert.Equal("Tester", saved.Title);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateInvalid()
        {
            // Arrange - persist user and create model with invalid state
            var user = new OfficeUser { Id = "profileUser2", UserName = "p2@test.com", Email = "p2@test.com", FirstName = "P", LastName = "Two", UserRole = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedUser);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Profile = new Profile() // missing required fields
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedUser.Id),
                new Claim(ClaimTypes.Name, persistedUser.UserName)
            }, "TestAuth"));
            createModel.PageContext = new PageContext { HttpContext = httpContext };
            createModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            createModel.ModelState.AddModelError("Profile.ProfileName", "Required");

            // Act
            var result = await createModel.OnPostAsync();

            // Assert - invalid model returns PageResult and nothing saved
            Assert.IsType<PageResult>(result);
            Assert.Empty(await _context.Summary.ToListAsync());
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

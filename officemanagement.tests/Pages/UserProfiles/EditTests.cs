using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class EditTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public EditTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnGetAsync_LoadsProfile_WhenIdIsValid()
        {
            // Arrange - create user and profile
            var user = new OfficeUser { Id = "edituser1", UserName = "edit1@test.com", Email = "edit1@test.com", FirstName = "Edit", LastName = "One", UserRole = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var profile = new Profile
            {
                ProfileName = "Edit",
                ProfileSurname = "One",
                Title = "Engineer",
                ProfileDescription = "Desc",
                Experience = "Exp",
                Education = "Ed",
                Skills = "S",
                Location = "Loc",
                Hobbies = "None",
                Notes = "Notes",
                DateJoined = DateTime.UtcNow.AddYears(-1),
                UserId = persistedUser.Id,
                User = persistedUser
            };
            _context.Summary.Add(profile);
            await _context.SaveChangesAsync();

            var persistedProfile = await _context.Summary.FirstAsync(p => p.UserId == persistedUser.Id);

            // Mock UserManager so GetUserAsync(User) returns persistedUser if page needs it
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new EditModel(_context, userManager.Object);

            // Provide a principal on PageContext
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedUser.Id),
                new Claim(ClaimTypes.Name, persistedUser.UserName)
            }, "TestAuth"));
            pageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            var result = await pageModel.OnGetAsync(persistedProfile.ProfileId);

            // Assert - page result and profile loaded
            Assert.IsType<PageResult>(result);
            Assert.NotNull(pageModel.Profile);
            Assert.Equal(persistedProfile.ProfileId, pageModel.Profile.ProfileId);
            Assert.Equal("Edit", pageModel.Profile.ProfileName);
        }

        [Fact]
        public async Task OnPostAsync_SavesChanges_WhenModelIsValid()
        {
            // Arrange - user and profile
            var user = new OfficeUser { Id = "edituser2", UserName = "edit2@test.com", Email = "edit2@test.com", FirstName = "Edit", LastName = "Two", UserRole = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var profile = new Profile
            {
                ProfileName = "Before",
                ProfileSurname = "Change",
                Title = "Dev",
                ProfileDescription = "old",
                Experience = "old",
                Education = "old",
                Skills = "old",
                Location = "old",
                Hobbies = "old",
                Notes = "old",
                DateJoined = DateTime.UtcNow.AddYears(-1),
                UserId = persistedUser.Id,
                User = persistedUser
            };
            _context.Summary.Add(profile);
            await _context.SaveChangesAsync();

            var persistedProfile = await _context.Summary.FirstAsync(p => p.UserId == persistedUser.Id);

            // Mock UserManager
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.Users).Returns(_context.Users);
            userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedUser);

            var pageModel = new EditModel(_context, userManager.Object)
            {
                // simulate the form-binding on post
                Profile = new Profile
                {
                    ProfileId = persistedProfile.ProfileId,
                    ProfileName = "After",
                    ProfileSurname = persistedProfile.ProfileSurname,
                    Title = "Senior Dev",
                    ProfileDescription = "updated",
                    Experience = "updated",
                    Education = "updated",
                    Skills = "C#",
                    Location = "Remote",
                    Hobbies = "none",
                    Notes = "updated",
                    DateJoined = persistedProfile.DateJoined,
                    UserId = persistedUser.Id
                }
            };

            // PageContext and TempData (Edit may set TempData)
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedUser.Id),
                new Claim(ClaimTypes.Name, persistedUser.UserName)
            }, "TestAuth"));
            pageModel.PageContext = new PageContext { HttpContext = httpContext };
            pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var updated = await _context.Summary.FindAsync(persistedProfile.ProfileId);
            Assert.NotNull(updated);
            Assert.Equal("After", updated.ProfileName);
            Assert.Equal("Senior Dev", updated.Title);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateInvalid()
        {
            // Arrange - create user + profile
            var user = new OfficeUser { Id = "edituser3", UserName = "edit3@test.com", Email = "edit3@test.com", FirstName = "Edit", LastName = "Three", UserRole = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var profile = new Profile
            {
                ProfileName = "Keep",
                ProfileSurname = "It",
                Title = "Dev",
                ProfileDescription = "desc",
                Experience = "exp",
                Education = "edu",
                Skills = "s",
                Location = "loc",
                Hobbies = "h",
                Notes = "n",
                DateJoined = DateTime.UtcNow.AddYears(-1),
                UserId = persistedUser.Id,
                User = persistedUser
            };
            _context.Summary.Add(profile);
            await _context.SaveChangesAsync();

            var persistedProfile = await _context.Summary.FirstAsync(p => p.UserId == persistedUser.Id);

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.Users).Returns(_context.Users);
            userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedUser);

            var pageModel = new EditModel(_context, userManager.Object)
            {
                Profile = new Profile
                {
                    ProfileId = persistedProfile.ProfileId,
                    // Missing required fields to simulate invalid model
                }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedUser.Id),
                new Claim(ClaimTypes.Name, persistedUser.UserName)
            }, "TestAuth"));
            pageModel.PageContext = new PageContext { HttpContext = httpContext };
            pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            pageModel.ModelState.AddModelError("Profile.ProfileName", "Required");

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert - invalid model returns PageResult and no changes were saved
            Assert.IsType<PageResult>(result);
            var unchanged = await _context.Summary.FindAsync(persistedProfile.ProfileId);
            Assert.Equal("Keep", unchanged.ProfileName);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

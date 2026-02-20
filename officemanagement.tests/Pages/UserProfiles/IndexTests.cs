using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task OnGetAsync_PopulatesProfiles_ForAdminShowsAllProfiles()
        {
            // Arrange - create two users and two profiles
            var admin = new OfficeUser { Id = "admin1", UserName = "admin@test.com", Email = "admin@test.com", FirstName = "Admin", LastName = "One", UserRole = "Admin" };
            var user = new OfficeUser { Id = "user1", UserName = "user@test.com", Email = "user@test.com", FirstName = "User", LastName = "One", UserRole = "User" };

            _context.Users.AddRange(admin, user);
            await _context.SaveChangesAsync();

            var persistedAdmin = await _context.Users.FirstAsync(u => u.Id == admin.Id);
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var profileAdmin = new Profile
            {
                ProfileName = "Admin",
                ProfileSurname = "One",
                Title = "Administrator",
                ProfileDescription = "Admin profile",
                Experience = "Many years",
                Education = "N/A",
                Skills = "All",
                Location = "HQ",
                Hobbies = "N/A",
                Notes = "None",
                DateJoined = DateTime.UtcNow.AddYears(-1),
                UserId = persistedAdmin.Id,
                User = persistedAdmin
            };

            var profileUser = new Profile
            {
                ProfileName = "User",
                ProfileSurname = "One",
                Title = "Developer",
                ProfileDescription = "User profile",
                Experience = "Some years",
                Education = "BS",
                Skills = "C#",
                Location = "Remote",
                Hobbies = "Coding",
                Notes = "Notes",
                DateJoined = DateTime.UtcNow.AddMonths(-6),
                UserId = persistedUser.Id,
                User = persistedUser
            };

            _context.Summary.AddRange(profileAdmin, profileUser);
            await _context.SaveChangesAsync();

            // Mock UserManager to return the persisted admin and mark as in Admin role
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedAdmin);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "Admin")).ReturnsAsync(true);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "DemoAdmin")).ReturnsAsync(false);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new IndexModel(_context, userManager.Object);

            // Provide a principal for GetUserAsync(User)
            var httpContext = new DefaultHttpContext();
            httpContext.User = TestClaimsPrincipalFactory.CreatePrincipalForUser(persistedAdmin);
            pageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            await pageModel.OnGetAsync();

            // Assert - an admin should see both profiles
            Assert.NotNull(pageModel.Profiles);
            var list = pageModel.Profiles.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, p => p.UserId == persistedAdmin.Id);
            Assert.Contains(list, p => p.UserId == persistedUser.Id);
            Assert.True(pageModel.IsAdmin || pageModel.IsDemoAdmin);
        }

        [Fact]
        public async Task OnGetAsync_PopulatesProfiles_ForNonAdmin_ShowsCurrentUserProfileFirst()
        {
            // Arrange - create a single user with profile
            var user = new OfficeUser { Id = "user2", UserName = "user2@test.com", Email = "user2@test.com", FirstName = "User", LastName = "Two", UserRole = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var persistedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);

            var profile = new Profile
            {
                ProfileName = "User",
                ProfileSurname = "Two",
                Title = "Engineer",
                ProfileDescription = "Profile desc",
                Experience = "3 years",
                Education = "BS",
                Skills = "Testing",
                Location = "City",
                Hobbies = "Hiking",
                Notes = "Notes",
                DateJoined = DateTime.UtcNow.AddMonths(-2),
                UserId = persistedUser.Id,
                User = persistedUser
            };
            _context.Summary.Add(profile);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedUser);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "Admin")).ReturnsAsync(false);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "DemoAdmin")).ReturnsAsync(false);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new IndexModel(_context, userManager.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.User = TestClaimsPrincipalFactory.CreatePrincipalForUser(persistedUser);
            pageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            await pageModel.OnGetAsync();

            // Assert - non-admin should still receive Profiles list and their own profile should be findable
            Assert.NotNull(pageModel.Profiles);
            Assert.Single(pageModel.Profiles.Where(p => p.UserId == persistedUser.Id));
            Assert.False(pageModel.IsAdmin);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }

    // small helper used by the tests to create a minimal ClaimsPrincipal for a user
    internal static class TestClaimsPrincipalFactory
    {
        public static System.Security.Claims.ClaimsPrincipal CreatePrincipalForUser(OfficeUser user)
        {
            var identity = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? string.Empty)
            }, "TestAuth");

            return new System.Security.Claims.ClaimsPrincipal(identity);
        }
    }
}

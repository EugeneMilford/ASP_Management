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
        public async Task OnPostAsync_Delete_RemovesProfile_WhenIdValid()
        {
            // Arrange - create admin user and a profile (note: controller uses _context.Summary)
            var admin = new OfficeUser { Id = "adminDel1", UserName = "ad1@test.com", Email = "ad1@test.com", FirstName = "Admin", LastName = "Del", UserRole = "Admin" };
            var profUser = new OfficeUser { Id = "profileDel1", UserName = "pdel1@test.com", Email = "pdel1@test.com", FirstName = "P", LastName = "Del", UserRole = "User" };
            _context.Users.AddRange(admin, profUser);
            await _context.SaveChangesAsync();

            var persistedAdmin = await _context.Users.FirstAsync(u => u.Id == admin.Id);
            var persistedProfUser = await _context.Users.FirstAsync(u => u.Id == profUser.Id);

            var profile = new Profile
            {
                ProfileName = "ToDelete",
                ProfileSurname = "Del",
                Title = "Title",
                ProfileDescription = "desc",
                Experience = "exp",
                Education = "edu",
                Skills = "skills",
                Location = "loc",
                Hobbies = "h",
                Notes = "n",
                DateJoined = DateTime.UtcNow.AddYears(-1),
                UserId = persistedProfUser.Id,
                User = persistedProfUser
            };

            // NOTE: your page model uses _context.Summary (not _context.Profiles), seed into Summary
            _context.Summary.Add(profile);
            await _context.SaveChangesAsync();

            var persistedProfile = await _context.Summary.FirstAsync(p => p.UserId == persistedProfUser.Id);

            // Provide a mock UserManager (constructor requires it) - DeleteModel doesn't call it in current implementation
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedAdmin);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "Admin")).ReturnsAsync(true);
            userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedAdmin);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new DeleteModel(_context, userManager.Object);

            // Provide PageContext and TempData if the page reads/writes TempData in other variants
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedAdmin.Id),
                new Claim(ClaimTypes.Name, persistedAdmin.UserName)
            }, "TestAuth"));
            pageModel.PageContext = new PageContext { HttpContext = httpContext };
            pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await pageModel.OnPostAsync(persistedProfile.ProfileId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var deleted = await _context.Summary.FindAsync(persistedProfile.ProfileId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsRedirect_WhenIdInvalid()
        {
            // Arrange: admin present but profile id doesn't exist
            var admin = new OfficeUser { Id = "adminDel2", UserName = "ad2@test.com", Email = "ad2@test.com", FirstName = "Admin", LastName = "Two", UserRole = "Admin" };
            _context.Users.Add(admin);
            await _context.SaveChangesAsync();
            var persistedAdmin = await _context.Users.FirstAsync(u => u.Id == admin.Id);

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(persistedAdmin);
            userManager.Setup(m => m.IsInRoleAsync(It.IsAny<OfficeUser>(), "Admin")).ReturnsAsync(true);
            userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(persistedAdmin);
            userManager.Setup(m => m.Users).Returns(_context.Users);

            var pageModel = new DeleteModel(_context, userManager.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, persistedAdmin.Id),
                new Claim(ClaimTypes.Name, persistedAdmin.UserName)
            }, "TestAuth"));
            pageModel.PageContext = new PageContext { HttpContext = httpContext };
            pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act - call delete with an ID that doesn't exist
            var result = await pageModel.OnPostAsync(99999);

            // Assert - current DeleteModel redirects even when the profile doesn't exist
            Assert.IsType<RedirectToPageResult>(result);

            // ensure nothing was removed (no profiles exist)
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
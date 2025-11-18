using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Pages.UserRoles;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserRoles
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
        public async Task OnGetAsync_ShowsOnlyPermanentForNonDemoUser()
        {
            var normalUser = new OfficeUser { Id = "user1", UserRole = "User" };
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(normalUser);

            // Add permanent, temporary and deleted roles
            _context.Roles.AddRange(
                new Models.Role
                {
                    Name = "Perm",
                    Surname = "Role",
                    Title = "Engineer",
                    EmailAddress = "perm@test.com",
                    RoleOfUser = "User",
                    IsTemporary = false,
                    TempUserId = null,
                    IsDeleted = false,
                    UserId = null
                },
                new Models.Role
                {
                    Name = "Temp",
                    Surname = "Role",
                    Title = "Contractor",
                    EmailAddress = "temp@test.com",
                    RoleOfUser = "User",
                    IsTemporary = true,
                    TempUserId = "demo1",
                    IsDeleted = false,
                    UserId = null
                },
                new Models.Role
                {
                    Name = "Deleted",
                    Surname = "Role",
                    Title = "Former",
                    EmailAddress = "deleted@test.com",
                    RoleOfUser = "User",
                    IsTemporary = false,
                    TempUserId = null,
                    IsDeleted = true,
                    UserId = null
                }
            );
            await _context.SaveChangesAsync();

            var pageModel = new IndexModel(_context, userManager.Object);

            await pageModel.OnGetAsync();

            Assert.NotNull(pageModel.userRoles);
            Assert.Single(pageModel.userRoles);
            Assert.Equal("Perm", pageModel.userRoles.First().Name);
        }

        [Fact]
        public async Task OnGetAsync_ShowsTemporaryForDemoAdmin()
        {
            var demoUser = new OfficeUser { Id = "demo1", UserRole = "DemoAdmin" };
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(demoUser, "DemoAdmin");

            _context.Roles.AddRange(
                new Models.Role { Name = "P1", Surname = "A", Title = "T", EmailAddress = "p1@test.com", RoleOfUser = "User", IsTemporary = false, IsDeleted = false },
                new Models.Role { Name = "TempByDemo", Surname = "B", Title = "T", EmailAddress = "td@test.com", RoleOfUser = "User", IsTemporary = true, TempUserId = "demo1", IsDeleted = false },
                new Models.Role { Name = "TempOther", Surname = "C", Title = "T", EmailAddress = "to@test.com", RoleOfUser = "User", IsTemporary = true, TempUserId = "other", IsDeleted = false }
            );
            await _context.SaveChangesAsync();

            var pageModel = new IndexModel(_context, userManager.Object);

            await pageModel.OnGetAsync();

            Assert.NotNull(pageModel.userRoles);
            Assert.Contains(pageModel.userRoles, r => r.Name == "P1");
            Assert.Contains(pageModel.userRoles, r => r.Name == "TempByDemo");
            Assert.DoesNotContain(pageModel.userRoles, r => r.Name == "TempOther");
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

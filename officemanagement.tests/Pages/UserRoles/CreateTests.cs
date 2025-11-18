using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserRoles;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserRoles
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
        public async Task OnPostAsync_AddsRole_ForAdminUser()
        {
            var adminUser = new OfficeUser { Id = "admin1", UserRole = "Admin", UserName = "admin@test.com", Email = "admin@test.com" };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(adminUser, "Admin");

            var createModel = new CreateModel(_context, userManager.Object)
            {
                roles = new Role
                {
                    Name = "New",
                    Surname = "Role",
                    Title = "Lead",
                    EmailAddress = "newrole@test.com",
                    RoleOfUser = "Admin"
                }
            };

            var result = await createModel.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
            var saved = await _context.Roles.FirstOrDefaultAsync(r => r.EmailAddress == "newrole@test.com");
            Assert.NotNull(saved);
            Assert.False(saved.IsTemporary);
            // Admin-created roles should have UserId (creator)
            Assert.Equal("admin1", saved.UserId);
        }

        [Fact]
        public async Task OnPostAsync_SetsTemporary_ForDemoAdmin()
        {
            var demoUser = new OfficeUser { Id = "demoadmin1", UserRole = "DemoAdmin", UserName = "demo@test.com", Email = "demo@test.com" };
            _context.Users.Add(demoUser);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(demoUser, "DemoAdmin");

            var createModel = new CreateModel(_context, userManager.Object)
            {
                roles = new Role
                {
                    Name = "TempRole",
                    Surname = "Role",
                    Title = "Contractor",
                    EmailAddress = "temprole@test.com",
                    RoleOfUser = "Demo"
                }
            };

            await createModel.OnPostAsync();

            var saved = await _context.Roles.FirstOrDefaultAsync(r => r.EmailAddress == "temprole@test.com");
            Assert.NotNull(saved);
            Assert.True(saved.IsTemporary);
            Assert.Equal("demoadmin1", saved.TempUserId);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.UserRoles;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserRoles
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
        public async Task OnGetAsync_ReturnsRole_WhenIdIsValid()
        {
            // arrange
            var role = new Role { Name = "R", Surname = "S", Title = "T", EmailAddress = "r@test.com", RoleOfUser = "User" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var editModel = new EditModel(_context);

            var result = await editModel.OnGetAsync(role.RoleId);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(editModel.Role);
            Assert.Equal(role.RoleId, editModel.Role.RoleId);
        }

        [Fact]
        public async Task OnPostAsync_UpdatesRole_WhenModelValid()
        {
            var role = new Role { Name = "Original", Surname = "S", Title = "T", EmailAddress = "orig@test.com", RoleOfUser = "User" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var editModel = new EditModel(_context) { Role = role };
            editModel.Role.Name = "Updated";

            var result = await editModel.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
            var updated = await _context.Roles.FindAsync(role.RoleId);
            Assert.Equal("Updated", updated.Name);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

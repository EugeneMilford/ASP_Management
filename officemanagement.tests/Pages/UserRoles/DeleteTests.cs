using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Data;
using OfficeManagement.Pages.UserRoles;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserRoles
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
        public async Task OnGetAsync_ReturnsRole_WhenIdIsValid()
        {
            var role = new Models.Role { Name = "R", Surname = "S", Title = "T", EmailAddress = "r@test.com", RoleOfUser = "User" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var deleteModel = new DeleteModel(_context);

            var result = await deleteModel.OnGetAsync(role.RoleId);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(deleteModel.Role);
            Assert.Equal(role.RoleId, deleteModel.Role.RoleId);
        }

        [Fact]
        public async Task OnPostAsync_RemovesRole_WhenIdIsValid()
        {
            var role = new Models.Role { Name = "ToDelete", Surname = "S", Title = "T", EmailAddress = "del@test.com", RoleOfUser = "User" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var deleteModel = new DeleteModel(_context);

            var result = await deleteModel.OnPostAsync(role.RoleId);

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Null(await _context.Roles.FindAsync(role.RoleId));
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

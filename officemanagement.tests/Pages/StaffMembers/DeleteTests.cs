using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Pages.StaffMembers;
using OfficeManagement.Tests.TestHelpers;
using Xunit;

namespace OfficeManagement.Tests.Pages.StaffMembers
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
        public async Task OnGetAsync_Delete_ReturnsStaffMember_WhenIdIsValid()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var staffId = _context.Personnel.First().ID;

            var deleteModel = new DeleteModel(_context);
            var result = await deleteModel.OnGetAsync(staffId);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(deleteModel.Staff);
            Assert.Equal(staffId, deleteModel.Staff.ID);
        }

        [Fact]
        public async Task OnPostAsync_Delete_RemovesStaffMember()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var staff = await _context.Personnel.FirstAsync();
            var staffId = staff.ID;
            var initial = await _context.Personnel.CountAsync();

            var deleteModel = new DeleteModel(_context);
            var result = await deleteModel.OnPostAsync(staffId);

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(initial - 1, await _context.Personnel.CountAsync());
            Assert.Null(await _context.Personnel.FindAsync(staffId));
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}

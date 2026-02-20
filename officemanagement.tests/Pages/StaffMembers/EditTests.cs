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
        public async Task OnGetAsync_ReturnsStaffMember_WhenIdIsValid()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var staffId = _context.Personnel.First().ID;

            var editModel = new EditModel(_context);

            var result = await editModel.OnGetAsync(staffId);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(editModel.Staff);
            Assert.Equal(staffId, editModel.Staff.ID);
        }

        [Fact]
        public async Task OnPostAsync_UpdatesStaffMember_WhenModelIsValid()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var staff = await _context.Personnel.FirstAsync();
            var originalName = staff.Name;

            var editModel = new EditModel(_context) { Staff = staff };
            editModel.Staff.Name = "UpdatedName";

            var result = await editModel.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
            var updated = await _context.Personnel.FindAsync(staff.ID);
            Assert.Equal("UpdatedName", updated.Name);
            Assert.NotEqual(originalName, updated.Name);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsNotFound_WhenConcurrencyConflictOccurs()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var staff = await _context.Personnel.FirstAsync();

            var editModel = new EditModel(_context) { Staff = staff };
            editModel.Staff.Name = "UpdatedName";

            _context.Personnel.Remove(staff);
            await _context.SaveChangesAsync();

            var result = await editModel.OnPostAsync();

            Assert.IsType<NotFoundResult>(result);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}
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
    public class DetailsTests : IDisposable
    {
        private readonly OfficeContext _context;
        private readonly SqlServerTestHelper.TestDatabase _testDb;

        public DetailsTests()
        {
            var masterConn = Environment.GetEnvironmentVariable("TEST_MASTER_CONN")
                             ?? "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

            var create = SqlServerTestHelper.CreateUniqueTestDatabaseAsync(masterConn).GetAwaiter().GetResult();
            _context = new OfficeContext(create.Options);
            _testDb = create.TestDb;
        }

        [Fact]
        public async Task OnGetAsync_Details_ReturnsStaffMember_WhenIdIsValid()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var staffId = _context.Personnel.First().ID;

            var detailsModel = new DetailsModel(_context);
            var result = await detailsModel.OnGetAsync(staffId);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(detailsModel.Staff);
            Assert.Equal(staffId, detailsModel.Staff.ID);
        }

        [Fact]
        public async Task OnGetAsync_Details_ReturnsAllStaffProperties()
        {
            await TestDataSeeder.SeedStaffAsync(_context);
            var expected = await _context.Personnel.FirstAsync();
            var detailsModel = new DetailsModel(_context);

            await detailsModel.OnGetAsync(expected.ID);

            Assert.Equal(expected.Name, detailsModel.Staff.Name);
            Assert.Equal(expected.Surname, detailsModel.Staff.Surname);
            Assert.Equal(expected.EmailAddress, detailsModel.Staff.EmailAddress);
            Assert.Equal(expected.Department, detailsModel.Staff.Department);
            Assert.Equal(expected.Title, detailsModel.Staff.Title);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}
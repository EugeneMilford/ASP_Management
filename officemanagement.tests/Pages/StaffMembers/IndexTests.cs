using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Pages.StaffMembers;
using OfficeManagement.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace OfficeManagement.Tests.Pages.StaffMembers
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
            var options = create.Options;
            _testDb = create.TestDb;

            _context = new OfficeContext(options);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsStaffMembers_ForAdminUser()
        {
            var adminUser = new OfficeUser { Id = "admin1", UserRole = "Admin" };
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(adminUser, "Admin");

            await TestDataSeeder.SeedStaffAsync(_context);

            var pageModel = new IndexModel(_context, userManager.Object);
            await pageModel.OnGetAsync();

            Assert.NotNull(pageModel.Staff);
            Assert.Equal(3, pageModel.Staff.Count);
        }

        [Fact]
        public async Task OnGetAsync_ExcludesDeletedStaffMembers()
        {
            var adminUser = new OfficeUser { Id = "admin1", UserRole = "Admin" };
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(adminUser, "Admin");

            // Add fully populated Staff objects so required non-nullable columns are set
            _context.Personnel.AddRange(
                new Models.Staff
                {
                    Name = "Active",
                    Surname = "User",
                    Gender = "Male",
                    Title = "Developer",
                    Department = "IT",
                    PhoneNumber = 123456789,
                    EmailAddress = "active@test.com",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    Address = "123 St",
                    DateJoined = DateTime.Now,
                    IsDeleted = false,
                    UserId = null
                },
                new Models.Staff
                {
                    Name = "Deleted",
                    Surname = "User",
                    Gender = "Female",
                    Title = "Manager",
                    Department = "HR",
                    PhoneNumber = 987654321,
                    EmailAddress = "deleted@test.com",
                    DateOfBirth = DateTime.Now.AddYears(-35),
                    Address = "456 St",
                    DateJoined = DateTime.Now,
                    IsDeleted = true,
                    UserId = null
                }
            );
            await _context.SaveChangesAsync();

            var pageModel = new IndexModel(_context, userManager.Object);

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.Single(pageModel.Staff);
            Assert.DoesNotContain(pageModel.Staff, s => s.IsDeleted);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsStaffMembers_ForDemoAdminUser()
        {
            var demoUser = new OfficeUser { Id = "demo1", UserRole = "DemoAdmin" };
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(demoUser, "DemoAdmin");

            await TestDataSeeder.SeedStaffAsync(_context);

            var pageModel = new IndexModel(_context, userManager.Object);
            await pageModel.OnGetAsync();

            Assert.NotNull(pageModel.Staff);
            Assert.True(pageModel.Staff.Count > 0);
        }

        [Fact]
        public async Task OnGetAsync_OrdersStaffMembers_BySurnameAndName()
        {
            var adminUser = new OfficeUser { Id = "admin1", UserRole = "Admin" };
            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(adminUser, "Admin");

            await TestDataSeeder.SeedStaffAsync(_context);

            var pageModel = new IndexModel(_context, userManager.Object);
            await pageModel.OnGetAsync();

            Assert.NotNull(pageModel.Staff);
            var first = pageModel.Staff.First();
            Assert.Equal("Doe", first.Surname);
        }

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _testDb?.Dispose();
        }
    }
}
using System;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using OfficeManagement.Pages.StaffMembers;
using OfficeManagement.Tests.TestHelpers;
using Xunit;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace OfficeManagement.Tests.Pages.StaffMembers
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
        public async Task OnPostAsync_AddsNewStaffMember_WhenModelIsValid()
        {
            var adminUser = new OfficeUser { Id = "admin1", UserRole = "Admin", UserName = "admin@test.com", Email = "admin@test.com" };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(adminUser, "Admin");

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Staff = new Staff
                {
                    Name = "New",
                    Surname = "Employee",
                    Gender = "Male",
                    Title = "Analyst",
                    Department = "Finance",
                    PhoneNumber = 555123456,
                    EmailAddress = "new.employee@test.com",
                    DateOfBirth = new DateTime(1995, 5, 15),
                    Address = "789 New St",
                    DateJoined = DateTime.Now
                }
            };

            var result = await createModel.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(1, await _context.Personnel.CountAsync());
            var saved = await _context.Personnel.FirstAsync();
            Assert.Equal("New", saved.Name);
            Assert.Equal("admin1", saved.UserId);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateIsInvalid()
        {
            var adminUser = new OfficeUser { Id = "admin2", UserRole = "Admin", UserName = "admin2@test.com", Email = "admin2@test.com" };
            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(adminUser, "Admin");

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Staff = new Staff()
            };
            createModel.ModelState.AddModelError("Name", "Required");

            var result = await createModel.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal(0, await _context.Personnel.CountAsync());
        }

        [Fact]
        public async Task OnPostAsync_SetsTemporaryFlags_ForDemoAdminUser()
        {
            var demoUser = new OfficeUser { Id = "demoadmin1", UserRole = "DemoAdmin", UserName = "demo@test.com", Email = "demo@test.com" };
            _context.Users.Add(demoUser);
            await _context.SaveChangesAsync();

            var userManager = MockUserManagerHelper.CreateMockUserManagerWithUser(demoUser, "DemoAdmin");

            var createModel = new CreateModel(_context, userManager.Object)
            {
                Staff = new Staff
                {
                    Name = "Temp",
                    Surname = "Employee",
                    Gender = "Male",
                    Title = "Contractor",
                    Department = "IT",
                    PhoneNumber = 999888777,
                    EmailAddress = "temp@test.com",
                    DateOfBirth = new DateTime(1988, 3, 10),
                    Address = "Temp Address",
                    DateJoined = DateTime.Now
                }
            };

            await createModel.OnPostAsync();

            var saved = await _context.Personnel.FirstAsync();
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

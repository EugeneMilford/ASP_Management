using System;
using OfficeManagement.Models;
using Xunit;

namespace OfficeManagement.Tests.Pages.StaffMembers
{
    public class StaffModelTests
    {
        [Fact]
        public void Staff_ShouldHaveRequiredProperties()
        {
            var staff = new Staff
            {
                ID = 1,
                Name = "John",
                Surname = "Doe",
                Gender = "Male",
                Title = "Developer",
                Department = "IT",
                PhoneNumber = 123456789,
                EmailAddress = "john.doe@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Address = "123 Main St",
                DateJoined = DateTime.Now,
                IsTemporary = false,
                IsDeleted = false
            };

            Assert.Equal(1, staff.ID);
            Assert.Equal("John", staff.Name);
            Assert.Equal("Doe", staff.Surname);
            Assert.Equal("Male", staff.Gender);
            Assert.Equal("Developer", staff.Title);
            Assert.Equal("IT", staff.Department);
            Assert.False(staff.IsDeleted);
        }

        [Fact]
        public void Staff_IsTemporary_DefaultsToFalse()
        {
            var staff = new Staff();
            Assert.False(staff.IsTemporary);
        }

        [Fact]
        public void Staff_IsDeleted_DefaultsToFalse()
        {
            var staff = new Staff();
            Assert.False(staff.IsDeleted);
        }

        [Fact]
        public void Staff_CanSetUserIdAndTempUserId()
        {
            var staff = new Staff { UserId = "user123", TempUserId = "tempuser456", IsTemporary = true };
            Assert.Equal("user123", staff.UserId);
            Assert.Equal("tempuser456", staff.TempUserId);
            Assert.True(staff.IsTemporary);
        }
    }
}

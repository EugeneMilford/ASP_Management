using System;
using OfficeManagement.Models;
using Xunit;

namespace OfficeManagement.Tests.Pages.StaffMembers
{
    public class DataValidationTests
    {
        [Fact]
        public void Staff_PhoneNumber_ShouldBeInteger()
        {
            var staff = new Staff
            {
                Name = "Test",
                Surname = "User",
                Gender = "Male",
                Title = "Developer",
                Department = "IT",
                PhoneNumber = 1234567890,
                EmailAddress = "test@test.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                Address = "Test Address",
                DateJoined = DateTime.Now
            };

            Assert.IsType<int>(staff.PhoneNumber);
        }

        [Fact]
        public void Staff_Dates_ShouldBeValid()
        {
            var dateOfBirth = new DateTime(1990, 5, 15);
            var dateJoined = DateTime.Now;
            var staff = new Staff { DateOfBirth = dateOfBirth, DateJoined = dateJoined };
            Assert.Equal(dateOfBirth, staff.DateOfBirth);
            Assert.Equal(dateJoined, staff.DateJoined);
            Assert.True(staff.DateJoined > staff.DateOfBirth);
        }
    }
}

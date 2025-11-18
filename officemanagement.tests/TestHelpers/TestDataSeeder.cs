using System;
using System.Threading.Tasks;
using OfficeManagement.Models;
using OfficeManagement.Data;

namespace OfficeManagement.Tests.TestHelpers
{
    public static class TestDataSeeder
    {
        public static async Task SeedStaffAsync(OfficeContext context)
        {
            context.Personnel.AddRange(
                new Staff
                {
                    Name = "John",
                    Surname = "Doe",
                    Gender = "Male",
                    Title = "Developer",
                    Department = "IT",
                    PhoneNumber = 123456789,
                    EmailAddress = "john.doe@test.com",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Address = "123 Main St",
                    DateJoined = DateTime.Now.AddYears(-2),
                    IsDeleted = false,
                    UserId = null
                },
                new Staff
                {
                    Name = "Jane",
                    Surname = "Smith",
                    Gender = "Female",
                    Title = "Manager",
                    Department = "HR",
                    PhoneNumber = 987654321,
                    EmailAddress = "jane.smith@test.com",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Address = "456 Oak Ave",
                    DateJoined = DateTime.Now.AddYears(-5),
                    IsDeleted = false,
                    UserId = null
                },
                new Staff
                {
                    Name = "Bob",
                    Surname = "Johnson",
                    Gender = "Male",
                    Title = "Analyst",
                    Department = "Finance",
                    PhoneNumber = 555555555,
                    EmailAddress = "bob.johnson@test.com",
                    DateOfBirth = new DateTime(1992, 8, 20),
                    Address = "789 Pine Rd",
                    DateJoined = DateTime.Now.AddYears(-1),
                    IsDeleted = false,
                    UserId = null
                }
            );
            await context.SaveChangesAsync();
        }
    }
}

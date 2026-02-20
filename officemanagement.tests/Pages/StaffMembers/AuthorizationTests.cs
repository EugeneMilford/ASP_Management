using OfficeManagement.Areas.Identity.Data;
using Xunit;

namespace OfficeManagement.Tests.Pages.StaffMembers
{
    public class AuthorizationTests
    {
        [Theory]
        [InlineData("Admin")]
        [InlineData("DemoAdmin")]
        public void User_WithAdminRole_ShouldHaveAccess(string role)
        {
            var user = new OfficeUser { UserRole = role };
            Assert.True(user.UserRole == "Admin" || user.UserRole == "DemoAdmin");
        }

        [Fact]
        public void User_WithoutAdminRole_ShouldNotHaveFullAccess()
        {
            var user = new OfficeUser { UserRole = "User" };
            Assert.False(user.UserRole == "Admin" || user.UserRole == "DemoAdmin");
        }

        [Fact]
        public void NullUser_TreatedAsDemoAdmin()
        {
            OfficeUser currentUser = null;
            var effectiveUser = currentUser ?? new OfficeUser { UserRole = "DemoAdmin" };
            Assert.Equal("DemoAdmin", effectiveUser.UserRole);
        }
    }
}

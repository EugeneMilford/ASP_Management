using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace OfficeManagement.Tests.TestHelpers
{
    public static class MockUserManagerHelper
    {
        public static Mock<UserManager<TUser>> CreateMockUserManager<TUser>() where TUser : class
        {
            var storeMock = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(
                storeMock.Object,
                null, null, null, null, null, null, null, null
            );

            mgr.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
               .ReturnsAsync((TUser?)null);

            mgr.Setup(x => x.IsInRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
               .ReturnsAsync(false);

            return mgr;
        }

        public static Mock<UserManager<TUser>> CreateMockUserManagerWithUser<TUser>(TUser user, params string[] roles) where TUser : class
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var mock = CreateMockUserManager<TUser>();

            mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            mock.Setup(m => m.IsInRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync((TUser u, string role) => roles != null && roles.Contains(role, StringComparer.OrdinalIgnoreCase));

            mock.Setup(m => m.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mock.Setup(m => m.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mock;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Areas.Identity.Data;

namespace OfficeManagement.Data
{
    public static class RoleSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<OfficeUser>>();

            string[] roleNames = { "User", "Admin", "Demo" };
            IdentityResult roleResult;

            // Ensure roles exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            const string adminEmail = "admin@example.com";
            const string adminPassword = "Password123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new OfficeUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdminResult.Succeeded)
                {
                    // Assign the admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create demo admin user
            var demoAdmin = new OfficeUser
            {
                UserName = "demoAdmin@example.com",
                Email = "demoAdmin@example.com",
                UserRole = "DemoAdmin"
            };

            // Create demo user
            var demoUser = new OfficeUser
            {
                UserName = "demoUser@example.com",
                Email = "demoUser@example.com",
                UserRole = "User"
            };

            if (!await userManager.Users.AnyAsync(u => u.Email == demoAdmin.Email))
            {
                var createDemoAdminResult = await userManager.CreateAsync(demoAdmin, "Password123!");
                if (createDemoAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(demoAdmin, "Demo");
                }
            }

            if (!await userManager.Users.AnyAsync(u => u.Email == demoUser.Email))
            {
                var createDemoUserResult = await userManager.CreateAsync(demoUser, "Password123!");
                if (createDemoUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(demoUser, "Demo");
                }
            }
        }
    }
}



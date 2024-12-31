using Microsoft.AspNetCore.Identity;
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

            string[] roleNames = { "User", "Admin", "Demo" }; // Added "Demo" role
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

            // Creating a demo user if it doesn't already exist
            const string demoEmail = "demo@example.com";
            const string demoPassword = "Password123!";

            var demoUser = await userManager.FindByEmailAsync(demoEmail);
            if (demoUser == null)
            {
                demoUser = new OfficeUser
                {
                    UserName = demoEmail,
                    Email = demoEmail,
                    UserRole = "Demo" // Set the UserRole property if applicable
                };

                var createDemoUserResult = await userManager.CreateAsync(demoUser, demoPassword);
                if (createDemoUserResult.Succeeded)
                {
                    // Assign the demo user the "Demo" role
                    await userManager.AddToRoleAsync(demoUser, "Demo");
                }
            }
        }
    }
}

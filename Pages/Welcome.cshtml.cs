using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeManagement.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly SignInManager<OfficeUser> _signInManager;
        private readonly UserManager<OfficeUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly OfficeContext _context;

        // Properties for role and profile completion status
        public string LoggedInRole { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsProfileComplete { get; set; }

        public WelcomeModel(
            SignInManager<OfficeUser> signInManager,
            UserManager<OfficeUser> userManager,
            RoleManager<IdentityRole> roleManager,
            OfficeContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/Index");

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Get the role of the logged-in user
                var roles = await _userManager.GetRolesAsync(user);
                LoggedInRole = roles.FirstOrDefault();  // Assuming the user has only one role

                // If the user is a demo user/admin, skip the profile check
                if (LoggedInRole == "DemoUser" || LoggedInRole == "DemoAdmin")
                {
                    IsProfileComplete = true; // Skip the profile check for demo users
                }
                else
                {
                    // Check if the profile exists for the logged-in user
                    IsProfileComplete = _context.Summary.Any(p => p.UserId == user.Id);
                }
            }
        }

        public async Task<IActionResult> OnPostDemoLoginAsync(string role, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Index");

            await CreateRoleIfNotExists("DemoUser");
            await CreateRoleIfNotExists("DemoAdmin");

            var demoEmail = role == "DemoAdmin" ? "demoAdmin@mail.com" : "demoUser@mail.com";
            var demoPassword = "Demo@123";

            var user = await _userManager.FindByEmailAsync(demoEmail);

            if (user == null)
            {
                user = new OfficeUser { UserName = demoEmail, Email = demoEmail, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, demoPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                    return Page();
                }

                await _userManager.AddToRoleAsync(user, role);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            LoggedInRole = role;

            return LocalRedirect(returnUrl);
        }

        private async Task CreateRoleIfNotExists(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
            }
        }
    }
}

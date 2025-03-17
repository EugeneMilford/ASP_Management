using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using System.Threading.Tasks;

namespace OfficeManagement.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly SignInManager<OfficeUser> _signInManager;
        private readonly UserManager<OfficeUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public WelcomeModel(SignInManager<OfficeUser> signInManager, UserManager<OfficeUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

       public async Task<IActionResult> OnPostDemoLoginAsync(string role)
{
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
            // Log error messages
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
            return Page();
        }

        await _userManager.AddToRoleAsync(user, role);
    }

    await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToPage("/index");
}

        private async Task CreateRoleIfNotExists(string roleName)
        {
            // Check if the role already exists, and if not, create it
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
            }
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace OfficeManagement.Pages
//{
//    public class WelcomeModel : PageModel
//    {
//        public void OnGet()
//        {
//        }
//    }
//}

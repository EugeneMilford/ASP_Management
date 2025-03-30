using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.Threading.Tasks;

namespace OfficeManagement.Pages.UserProfiles
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public CreateModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Profile Profile { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool MyProfile { get; set; } = false;

        public IActionResult OnGet()
        {
            if (MyProfile)
            {
                Profile = new Profile();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            Profile.UserId = currentUser.Id;

            _context.Summary.Add(Profile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserRoles
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public CreateModel(
            OfficeContext context,
            UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Role roles { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var currentUser = await _userManager.GetUserAsync(User); // _userManager instead of UserManager

            if (await _userManager.IsInRoleAsync(currentUser, "DemoAdmin")) // _userManager instead of UserManager
            {
                roles.IsTemporary = true;
                roles.TempUserId = currentUser.Id;
            }

            _context.Roles.Add(roles);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}


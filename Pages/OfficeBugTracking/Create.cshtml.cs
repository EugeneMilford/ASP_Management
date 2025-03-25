using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.OfficeBugTracking
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public BugTracking bugTracking { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var currentUser = await _userManager.GetUserAsync(User); // _userManager instead of UserManager

            if (await _userManager.IsInRoleAsync(currentUser, "DemoAdmin")) // _userManager instead of UserManager
            {
                bugTracking.IsTemporary = true;
                bugTracking.TempUserId = currentUser.Id;
            }

            _context.Bugs.Add(bugTracking);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.StaffMembers
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
        public Staff Staff { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var currentUser = await _userManager.GetUserAsync(User); 

            if (await _userManager.IsInRoleAsync(currentUser, "DemoAdmin")) 
            {
                Staff.IsTemporary = true;
                Staff.TempUserId = currentUser.Id;
            }

            _context.Personnel.Add(Staff);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
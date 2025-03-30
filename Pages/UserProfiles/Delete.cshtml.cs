using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.Threading.Tasks;

namespace OfficeManagement.Pages.UserProfiles
{
    [Authorize(Roles = "Admin")] // Only Admins can delete
    public class DeleteModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public DeleteModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Profile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile = await _context.Summary.FirstOrDefaultAsync(m => m.ProfileId == id);

            if (Profile == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile = await _context.Summary.FindAsync(id);

            if (Profile != null)
            {
                _context.Summary.Remove(Profile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

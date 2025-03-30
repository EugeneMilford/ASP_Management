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
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public DetailsModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Profile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isDemoAdmin = await _userManager.IsInRoleAsync(currentUser, "DemoAdmin");

            Profile = await _context.Summary
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProfileId == id);

            if (Profile == null)
            {
                return NotFound();
            }

            // Only allow if user is admin/demo-admin OR owns the profile
            if (!isAdmin && !isDemoAdmin && Profile.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return Page();
        }
    }
}

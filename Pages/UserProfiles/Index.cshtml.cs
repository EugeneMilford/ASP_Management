using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OfficeManagement.Pages.UserProfiles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public IndexModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Profile> Profiles { get; set; }
        public bool IsAdmin { get; private set; }
        public bool IsDemoAdmin { get; private set; }

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            IsAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin") || currentUser.UserRole == "Admin";
            IsDemoAdmin = await _userManager.IsInRoleAsync(currentUser, "DemoAdmin") || currentUser.UserRole == "DemoAdmin";

            if (IsAdmin || IsDemoAdmin)
            {
                // Both Admins and DemoAdmins see all profiles
                Profiles = await _context.Summary
                    .Include(p => p.User)
                    .ToListAsync();
            }
            else
            {
                // Regular users see only their own profile
                var userId = currentUser.Id;
                Profiles = await _context.Summary
                    .Where(p => p.UserId == userId)
                    .Include(p => p.User)
                    .ToListAsync();
            }
        }
    }
}


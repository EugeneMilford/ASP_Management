using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeManagement.Pages.UserRoles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        private static List<Role> _demoRole = new List<Role>();

        public IndexModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager
        }

        public IList<Role> userRoles { get; set; }

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isDemoAdmin = currentUser != null &&
                await _userManager.IsInRoleAsync(currentUser, "DemoAdmin");

            var query = _context.Roles
                .Where(s => !s.IsDeleted) // Exclude deleted records for everyone
                .AsQueryable();

            if (isDemoAdmin)
            {
                // Show temporary entries created by this demo admin
                query = query.Where(s =>
                    (!s.IsTemporary && s.TempUserId == null) ||
                    (s.TempUserId == currentUser.Id)
                );
            }
            else
            {
                // Show only permanent entries
                query = query.Where(s => !s.IsTemporary && s.TempUserId == null);
            }

            userRoles = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateDemoAsync(Role userRole)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                userRole.RoleId = _demoRole.Any() ? _demoRole.Max(s => s.RoleId) + 1 : 1; // Assign a temporary ID
                _demoRole.Add(userRole);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditDemoAsync(int id, Role userRole)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                var existingRole = _demoRole.FirstOrDefault(s => s.RoleId == id);
                if (existingRole != null)
                {
                    existingRole.Name = userRole.Name;
                    existingRole.Surname = userRole.Surname;
                    existingRole.Title = userRole.Title;
                    existingRole.EmailAddress = userRole.EmailAddress;
                    existingRole.RoleOfUser = userRole.RoleOfUser;
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteDemoAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Use UserManager to get user

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                var roleToRemove = _demoRole.FirstOrDefault(s => s.RoleId == id);
                if (roleToRemove != null)
                {
                    _demoRole.Remove(roleToRemove);
                }
            }

            return RedirectToPage();
        }

        public static void ClearDemoRoles()
        {
            _demoRole.Clear();
        }
    }
}


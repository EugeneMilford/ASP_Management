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

namespace OfficeManagement.Pages.StaffMembers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager; // Add UserManager

        private static List<Staff> _demoStaff = new List<Staff>(); // In-memory storage for demo admins

        public IndexModel(OfficeContext context, UserManager<OfficeUser> userManager) // Include UserManager in constructor
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager
        }

        public IList<Staff> Staff { get; set; }

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isDemoAdmin = currentUser != null &&
                await _userManager.IsInRoleAsync(currentUser, "DemoAdmin");

            var query = _context.Personnel
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

            Staff = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateDemoAsync(Staff staff)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Use UserManager to get user

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                staff.ID = _demoStaff.Any() ? _demoStaff.Max(s => s.ID) + 1 : 1; // Assign a temporary ID
                _demoStaff.Add(staff);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditDemoAsync(int id, Staff staff)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Use UserManager to get user

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                var existingStaff = _demoStaff.FirstOrDefault(s => s.ID == id);
                if (existingStaff != null)
                {
                    // Update properties based on the edited staff object
                    existingStaff.Name = staff.Name;
                    existingStaff.Surname = staff.Surname;
                    existingStaff.Gender = staff.Gender;
                    existingStaff.Department = staff.Department;
                    existingStaff.Title = staff.Title;
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteDemoAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Use UserManager to get user

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                var staffToRemove = _demoStaff.FirstOrDefault(s => s.ID == id);
                if (staffToRemove != null)
                {
                    _demoStaff.Remove(staffToRemove);
                }
            }

            return RedirectToPage();
        }

        public static void ClearDemoStaff()
        {
            _demoStaff.Clear();
        }
    }
}

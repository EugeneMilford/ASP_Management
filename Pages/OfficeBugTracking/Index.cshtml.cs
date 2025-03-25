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

namespace OfficeManagement.Pages.OfficeBugTracking
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        private static List<BugTracking> _demoBugs = new List<BugTracking>();

        public IndexModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<BugTracking> BugTracking { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isDemoAdmin = currentUser != null &&
                await _userManager.IsInRoleAsync(currentUser, "DemoAdmin");

            var query = _context.Bugs
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

            BugTracking = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateDemoAsync(BugTracking bugs)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                bugs.TicketId = _demoBugs.Any() ? _demoBugs.Max(s => s.TicketId) + 1 : 1; // Assign a temporary ID
                _demoBugs.Add(bugs);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditDemoAsync(int id, BugTracking bugs)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                var existingBugs = _demoBugs.FirstOrDefault(s => s.TicketId == id);
                if (existingBugs != null)
                {
                    existingBugs.Title = bugs.Title;
                    existingBugs.Priority = bugs.Priority;
                    existingBugs.Project = bugs.Project;
                    existingBugs.Description = bugs.Description;
                    existingBugs.CreatedDate = bugs.CreatedDate;
                    existingBugs.Status = bugs.Status;
                    existingBugs.Submitter = bugs.Submitter;
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteDemoAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User); // Use UserManager to get user

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "DemoAdmin"))
            {
                var bugsToRemove = _demoBugs.FirstOrDefault(s => s.TicketId == id);
                if (bugsToRemove != null)
                {
                    _demoBugs.Remove(bugsToRemove);
                }
            }

            return RedirectToPage();
        }

        public static void ClearDemoBugs()
        {
            _demoBugs.Clear();
        }
    }
}


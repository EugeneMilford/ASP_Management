using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;

namespace OfficeManagement.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<OfficeUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<OfficeUser> _userManager; 
        private readonly OfficeContext _context; 

        public LogoutModel(
            SignInManager<OfficeUser> signInManager,
            ILogger<LogoutModel> logger,
            UserManager<OfficeUser> userManager,
            OfficeContext context) 
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isDemoAdmin = false;

            if (user != null)
            {
                isDemoAdmin = await _userManager.IsInRoleAsync(user, "DemoAdmin");
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (isDemoAdmin && user != null)
            {
                // Remove temporary Staff Member entries
                var tempActivities = await _context.Activities
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Remove temporary Staff Member entries
                var tempStaffEntries = await _context.Personnel
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Remove temporary User Role entries
                var tempRoleEntries = await _context.Roles
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Remove temporary Bug Tracking entries
                var tempBugEntries = await _context.Bugs
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Remove temporary User Profiles entries
                var tempUserProfiles = await _context.Summary
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Remove temporary User Profiles entries
                var tempAssignments = await _context.Assignments
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Remove temporary Office Project entries
                var tempProjects = await _context.Projects
                    .Where(s => s.TempUserId == user.Id)
                    .ToListAsync();

                // Restore soft-deleted staff entries
                var softStaffDeleted = await _context.Personnel
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                // Restore soft-deleted assignment entries
                var softAssignmentDeleted = await _context.Assignments
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                // Restore soft-deleted role entries
                var softRolesDeleted = await _context.Roles
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                // Restore soft-deleted bug entries
                var softBugsDeleted = await _context.Bugs
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                // Restore soft-deleted Profile entries
                var softProfileDeleted = await _context.Summary
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                // Restore soft-deleted Project entries
                var softProjectsDeleted = await _context.Projects
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                // Restore soft-deleted Project entries
                var softActivitiesDeleted = await _context.Activities
                    .Where(s => s.TempUserId == user.Id && s.IsDeleted)
                    .ToListAsync();

                foreach (var entry in softStaffDeleted)
                {
                    entry.IsDeleted = false;
                    entry.TempUserId = null;
                }

                foreach (var entry in softRolesDeleted)
                {
                    entry.IsDeleted = false;
                    entry.TempUserId = null;
                }

                // Remove temporary created entries
                _context.Personnel.RemoveRange(tempStaffEntries);
                await _context.SaveChangesAsync();

                // Remove temporary created entries
                _context.Roles.RemoveRange(tempRoleEntries);
                await _context.SaveChangesAsync();

                // Remove temporary created entries
                _context.Bugs.RemoveRange(tempBugEntries);
                await _context.SaveChangesAsync();

                // Remove temporary created Assignment entries
                _context.Assignments.RemoveRange(tempAssignments);
                await _context.SaveChangesAsync();

                // Remove temporary created Activity entries
                _context.Activities.RemoveRange(tempActivities);
                await _context.SaveChangesAsync();

                // Remove temporary created Profile entries
                _context.Summary.RemoveRange(tempUserProfiles);
                await _context.SaveChangesAsync();

                // Remove temporary created Profile entries
                _context.Projects.RemoveRange(tempProjects);
                await _context.SaveChangesAsync();
            }

            return LocalRedirect(Url.Content("~/"));
        }
    }
}

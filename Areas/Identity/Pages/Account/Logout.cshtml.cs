// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

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
        private readonly UserManager<OfficeUser> _userManager; // Add UserManager to check user roles
        private readonly OfficeContext _context; // Inject OfficeContext

        public LogoutModel(
            SignInManager<OfficeUser> signInManager,
            ILogger<LogoutModel> logger,
            UserManager<OfficeUser> userManager,
            OfficeContext context) // Inject OfficeContext
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

                // Restore soft-deleted staff entries
                var softStaffDeleted = await _context.Personnel
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
            }

            return LocalRedirect(Url.Content("~/"));
        }
    }
}

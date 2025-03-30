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
    public class EditModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public EditModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Profile Profile { get; set; }

        public bool CanEdit { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isDemoAdmin = await _userManager.IsInRoleAsync(currentUser, "DemoAdmin");

            Profile = await _context.Summary.FirstOrDefaultAsync(m => m.ProfileId == id);

            if (Profile == null)
            {
                return NotFound();
            }

            // Set CanEdit flag
            CanEdit = isAdmin || isDemoAdmin || Profile.UserId == currentUser.Id;

            if (!CanEdit)
            {
                return Page(); // Show the access denied message in the view
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isDemoAdmin = await _userManager.IsInRoleAsync(currentUser, "DemoAdmin");

            var existingProfile = await _context.Summary.FindAsync(Profile.ProfileId);

            if (existingProfile == null)
            {
                return NotFound();
            }

            // Verify edit permission again on post
            CanEdit = isAdmin || isDemoAdmin || existingProfile.UserId == currentUser.Id;
            if (!CanEdit)
            {
                return Forbid();
            }

            // Update editable fields
            existingProfile.ProfileName = Profile.ProfileName;
            existingProfile.ProfileSurname = Profile.ProfileSurname;
            existingProfile.ProfileDescription = Profile.ProfileDescription;
            existingProfile.Title = Profile.Title;
            existingProfile.Experience = Profile.Experience;
            existingProfile.Education = Profile.Education;
            existingProfile.Skills = Profile.Skills;
            existingProfile.Location = Profile.Location;
            existingProfile.Hobbies = Profile.Hobbies;
            existingProfile.Notes = Profile.Notes;
            existingProfile.DateJoined = Profile.DateJoined;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(Profile.ProfileId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProfileExists(int id)
        {
            return _context.Summary.Any(e => e.ProfileId == id);
        }
    }
}
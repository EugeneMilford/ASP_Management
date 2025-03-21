using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OfficeManagement.Models;
using OfficeManagement.Areas.Identity.Data;
using System.Linq;
using System.Threading.Tasks;
using OfficeManagement.Data;

namespace OfficeManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<OfficeUser> _userManager;
        private readonly OfficeContext _context;

        public IndexModel(
            ILogger<IndexModel> logger,
            UserManager<OfficeUser> userManager,
            OfficeContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public string LoggedInRole { get; set; }
        public bool IsProfileComplete { get; set; }

        public async Task OnGetAsync()
        {
            // Retrieve the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Check the user's role
                var roles = await _userManager.GetRolesAsync(user);
                LoggedInRole = roles.FirstOrDefault();  // Get the first role, assuming the user has only one role

                // Check if the user's profile is complete
                IsProfileComplete = _context.Summary
                    .Any(p => p.UserId == user.Id);
            }
            else
            {
                IsProfileComplete = false;
            }
            TempData["LoggedInRole"] = LoggedInRole;
        }
    }
}

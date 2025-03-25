// IndexModel.cshtml.cs
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Models;
using Microsoft.AspNetCore.Identity;
using OfficeManagement.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfficeManagement.Pages.UserMessages
{
    public class IndexModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public IndexModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Message> Messages { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            Messages = await _context.Messages
                .Include(m => m.FromUser)
                .Where(m => m.ToUserId == currentUser.Id)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int messageId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && m.ToUserId == currentUser.Id);

            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                StatusMessage = "Message deleted successfully.";
            }

            return RedirectToPage();
        }
    }
}


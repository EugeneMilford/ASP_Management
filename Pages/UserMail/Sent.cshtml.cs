using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserMail
{
    [Authorize]
    public class SentModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public SentModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Mail> SentMails { get; set; }
        public int InboxCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Get sent emails (where current user is the sender)
            SentMails = await _context.Mails
                .Where(m => m.Sender == currentUser.Email)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            // Get count of received emails for inbox badge
            InboxCount = await _context.Mails
                .Where(m => m.UserId == currentUser.Id)
                .CountAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var mail = await _context.Mails
                .Where(m => m.MailId == id && m.Sender == currentUser.Email)
                .FirstOrDefaultAsync();

            if (mail != null)
            {
                _context.Mails.Remove(mail);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Mail successfully deleted.";
            }
            else
            {
                TempData["StatusMessage"] = "Mail not found or you don't have permission to delete it.";
            }

            return RedirectToPage();
        }
    }
}
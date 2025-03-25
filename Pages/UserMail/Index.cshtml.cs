// IndexModel.cshtml.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserMail
{
    public class IndexModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public IndexModel(OfficeContext context, UserManager<OfficeUser>
            userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Mail>
            InboxMails
        { get; set; }

        public async Task<IActionResult>
            OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            InboxMails = await _context.Mails
            .Where(m => m.UserId == currentUser.Id)
            .OrderByDescending(m => m.CreatedDate)
            .ToListAsync();

            return Page();
        }

        public async Task<IActionResult>
            OnPostDeleteAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var mail = await _context.Mails
            .FirstOrDefaultAsync(m => m.MailId == id && m.UserId == currentUser.Id);

            if (mail != null)
            {
                _context.Mails.Remove(mail);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult>
            OnPostMarkSpamAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var mail = await _context.Mails
            .FirstOrDefaultAsync(m => m.MailId == id && m.UserId == currentUser.Id);

            if (mail != null)
            {
                mail.IsSpam = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
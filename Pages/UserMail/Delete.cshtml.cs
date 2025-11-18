using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserMail
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeContext _context;

        public DeleteModel(OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Mail Mail { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Mails == null)
            {
                return NotFound();
            }

            var mail = await _context.Mails.FirstOrDefaultAsync(m => m.MailId == id);

            if (mail == null)
            {
                return NotFound();
            }

            Mail = mail;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Mails == null)
            {
                return NotFound();
            }

            var mail = await _context.Mails.FindAsync(id);

            if (mail == null)
            {
                // Explicitly return NotFound when the entity doesn't exist.
                return NotFound();
            }

            _context.Mails.Remove(mail);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

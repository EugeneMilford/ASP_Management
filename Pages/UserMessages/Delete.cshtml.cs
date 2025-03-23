using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.Threading.Tasks;

namespace OfficeManagement.Pages.UserMessages
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeContext _context;

        public DeleteModel(OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == id);

            if (Message == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Message = await _context.Messages.FindAsync(id);

            if (Message != null)
            {
                _context.Messages.Remove(Message);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}


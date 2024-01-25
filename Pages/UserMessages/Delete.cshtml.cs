using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserMessages
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DeleteModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }
            else 
            {
                Message = message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }
            var message = await _context.Messages.FindAsync(id);

            if (message != null)
            {
                Message = message;
                _context.Messages.Remove(Message);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

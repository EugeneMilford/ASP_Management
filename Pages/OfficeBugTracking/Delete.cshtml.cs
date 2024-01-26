using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.OfficeBugTracking
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DeleteModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
      public BugTracking BugTracking { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Bugs == null)
            {
                return NotFound();
            }

            var bugtracking = await _context.Bugs.FirstOrDefaultAsync(m => m.TicketId == id);

            if (bugtracking == null)
            {
                return NotFound();
            }
            else 
            {
                BugTracking = bugtracking;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Bugs == null)
            {
                return NotFound();
            }
            var bugtracking = await _context.Bugs.FindAsync(id);

            if (bugtracking != null)
            {
                BugTracking = bugtracking;
                _context.Bugs.Remove(BugTracking);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

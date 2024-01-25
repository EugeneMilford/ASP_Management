using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.Tasks
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DeleteModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Assignment Assignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FirstOrDefaultAsync(m => m.AssignmentId == id);

            if (assignment == null)
            {
                return NotFound();
            }
            else 
            {
                Assignment = assignment;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment != null)
            {
                Assignment = assignment;
                _context.Assignments.Remove(Assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

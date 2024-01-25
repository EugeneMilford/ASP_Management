using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.OfficeEvents
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DeleteModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Activity Activity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Activities == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FirstOrDefaultAsync(m => m.ActivityId == id);

            if (activity == null)
            {
                return NotFound();
            }
            else 
            {
                Activity = activity;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Activities == null)
            {
                return NotFound();
            }
            var activity = await _context.Activities.FindAsync(id);

            if (activity != null)
            {
                Activity = activity;
                _context.Activities.Remove(Activity);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

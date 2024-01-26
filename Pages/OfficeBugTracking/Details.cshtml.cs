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
    public class DetailsModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DetailsModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

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
    }
}

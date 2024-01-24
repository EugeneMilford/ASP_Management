using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.StaffMembers
{
    public class DetailsModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DetailsModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

      public Staff Staff { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Personnel == null)
            {
                return NotFound();
            }

            var staff = await _context.Personnel.FirstOrDefaultAsync(m => m.ID == id);
            if (staff == null)
            {
                return NotFound();
            }
            else 
            {
                Staff = staff;
            }
            return Page();
        }
    }
}

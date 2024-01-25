using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserProfiles
{
    public class DeleteModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DeleteModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Profile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Profiles == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(m => m.ProfileId == id);

            if (profile == null)
            {
                return NotFound();
            }
            else 
            {
                Profile = profile;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Profiles == null)
            {
                return NotFound();
            }
            var profile = await _context.Profiles.FindAsync(id);

            if (profile != null)
            {
                Profile = profile;
                _context.Profiles.Remove(Profile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

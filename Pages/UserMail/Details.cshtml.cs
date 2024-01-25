using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Pages.UserMail
{
    public class DetailsModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public DetailsModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

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
            else 
            {
                Mail = mail;
            }
            return Page();
        }
    }
}

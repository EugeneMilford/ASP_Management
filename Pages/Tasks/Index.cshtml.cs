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
    public class IndexModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public IndexModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        public IList<Assignment> Assignment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Assignments != null)
            {
                Assignment = await _context.Assignments.ToListAsync();
            }
        }
    }
}

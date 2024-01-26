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
    public class IndexModel : PageModel
    {
        private readonly OfficeManagement.Data.OfficeContext _context;

        public IndexModel(OfficeManagement.Data.OfficeContext context)
        {
            _context = context;
        }

        public IList<BugTracking> BugTracking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Bugs != null)
            {
                BugTracking = await _context.Bugs.ToListAsync();
            }
        }
    }
}

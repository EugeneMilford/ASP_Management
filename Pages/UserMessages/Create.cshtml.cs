// CreateModel.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Models;
using System.Threading.Tasks;
using OfficeManagement.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OfficeManagement.Pages.UserMessages
{
    public class CreateModel : PageModel
    {
        private readonly OfficeContext _context;
        private readonly UserManager<OfficeUser> _userManager;

        public CreateModel(OfficeContext context, UserManager<OfficeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public MessageInputModel Input { get; set; }

        public class MessageInputModel
        {
            public string ToUserId { get; set; }
            public string Content { get; set; }
        }

        public SelectList UserList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            // Get all users except current user
            var users = await _userManager.Users
                .Where(u => u.Id != currentUser.Id)
                .Select(u => new
                {
                    Id = u.Id,
                    DisplayName = $"{u.FirstName} {u.LastName} ({u.Email})"
                })
                .ToListAsync();

            UserList = new SelectList(users, "Id", "DisplayName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Reload the user list
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            // Create and save the message
            var message = new Message
            {
                FromUserId = currentUser.Id,
                ToUserId = Input.ToUserId,
                Content = Input.Content,
                SentDate = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message sent successfully!";
            return RedirectToPage("./Index");
        }
    }
}

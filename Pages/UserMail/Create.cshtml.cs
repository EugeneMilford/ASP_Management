// CreateModel.cshtml.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Data;
using OfficeManagement.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OfficeManagement.Pages.UserMail
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
        public MailInputModel Input { get; set; }

        public SelectList UserList { get; set; }

        public class MailInputModel
        {
            [Required(ErrorMessage = "Please select a recipient")]
            [Display(Name = "To")]
            public string UserId { get; set; }

            [Required(ErrorMessage = "Subject is required")]
            [StringLength(100, ErrorMessage = "Subject cannot exceed 100 characters")]
            [Display(Name = "Subject")]
            public string MailTopic { get; set; }

            [Required(ErrorMessage = "Content is required")]
            [Display(Name = "Content")]
            public string MailContent { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var users = await _userManager.Users
                .Where(u => u.Id != currentUser.Id)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
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
                await OnGetAsync();
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var mail = new Mail
            {
                MailTopic = Input.MailTopic,
                MailContent = Input.MailContent,
                Sender = currentUser.Email,
                UserId = Input.UserId,
                CreatedDate = DateTime.UtcNow,
                IsSpam = false
            };

            _context.Mails.Add(mail);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
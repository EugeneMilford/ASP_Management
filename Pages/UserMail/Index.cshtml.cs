using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace OfficeManagement.Pages.UserMail
{
    public class IndexModel : PageModel
    {
        public IList<Mail> Mail { get; set; } = default!;

        public void OnGet()
        {
            // This would typically come from a database context
            Mail = new List<Mail>
            {
                new Mail { MailId = 1, MailTopic = "Meeting Reminder", MailContent = "Don't forget our meeting tomorrow.", Sender = "John Doe", CreatedDate = DateTime.Now.AddDays(-1) },
                new Mail { MailId = 2, MailTopic = "Project Update", MailContent = "The project is progressing well.", Sender = "Jane Smith", CreatedDate = DateTime.Now.AddDays(-2) },
            };
        }
    }
}
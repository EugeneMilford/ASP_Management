using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace OfficeManagement.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Placeholder email-sending logic
            Console.WriteLine($"Sending email to {email} with subject {subject}");
            return Task.CompletedTask; 
        }
    }
}

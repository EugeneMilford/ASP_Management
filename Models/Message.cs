using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        [Required]
        [Display(Name = "Sender")]
        public string MessageSender { get; set; }
        [Required]
        [Display(Name = "Message")]
        public string MessageBody { get; set; }
        [Required]
        [Display(Name = "Time")]
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }

        public OfficeUser User { get; set; }
    }
}

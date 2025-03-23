using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeManagement.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        [Required]
        [ForeignKey("FromUser")]
        public string FromUserId { get; set; }

        [Required]
        [ForeignKey("ToUser")]
        public string ToUserId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentDate { get; set; }

        // Navigation properties for sender and recipient
        public virtual OfficeUser FromUser { get; set; }
        public virtual OfficeUser ToUser { get; set; }
    }
}

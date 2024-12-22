using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class BugTracking
    {
        [Key]
        public int TicketId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Priority { get; set; }
        [Required]
        public string Project { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Submitter { get; set; }
        public string UserId { get; set; }

        public OfficeUser User { get; set; }
    }
}

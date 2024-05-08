using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        [Required]
        [Display(Name = "Assignment")]
        public string AssignmentName { get; set; }
        [Display(Name = "Description")]
        [Required]
        public string AssignmentDescription { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
    }
}

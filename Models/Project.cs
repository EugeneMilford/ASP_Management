using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        [Required]
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        [Required]
        [Display(Name = "Project Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Display(Name = "Project User")]
        public string ProjectUser { get; set; }
        public string UserId { get; set; }
        public OfficeUser User { get; set; }
    }
}

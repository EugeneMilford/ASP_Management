using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        [Required]
        public string ProfileName { get; set; }
        [Required]
        public string ProfileDescription { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Experience { get; set; }
        [Required]
        public string Education { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Notes { get; set; }
        [Required]
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }
    }
}

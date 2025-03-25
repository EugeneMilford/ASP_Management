using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string ProfileName { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string ProfileSurname { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string ProfileDescription { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Experience { get; set; }
        [Required]
        public string Education { get; set; }
        [Required]
        public string Skills { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Hobbies { get; set; }
        [Required]
        public string Notes { get; set; }
        [Required]
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }
        public string UserId { get; set; }

        public OfficeUser User { get; set; }
    }
}


using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Staff
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public int PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }
        public string UserId { get; set; }
        public OfficeUser User { get; set; }
        public bool IsTemporary { get; set; }
        public string? TempUserId { get; set; } // user reference
        public bool IsDeleted { get; set; }
    }
}


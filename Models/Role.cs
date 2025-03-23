using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Title { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "User Role")]
        public string RoleOfUser { get; set; }
        public string UserId { get; set; }
        public OfficeUser User { get; set; }
        public bool IsTemporary { get; set; }
        public string? TempUserId { get; set; } // user reference
        public bool IsDeleted { get; set; }
    }
}

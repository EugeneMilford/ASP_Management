using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        [Required]
        [Display(Name = "Event")]
        public string EventName { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string EventDescription { get; set; }
        [Required]
        [Display(Name = "Time")]
        public DateTime EventTime { get; set; }
        [Required]
        [Display(Name = "Users Assigned")]
        public string UsersAssigned { get; set; }
    }
}

using OfficeManagement.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class CalendarEvent
    {
        [Key]
        public int CalendarId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UserId { get; set; }
        public OfficeUser User { get; set; }
    }
}


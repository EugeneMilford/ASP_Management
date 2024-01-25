namespace OfficeManagement.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime EventTime { get; set; }
        public string UsersAssigned { get; set; }
    }
}

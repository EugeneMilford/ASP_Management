namespace OfficeManagement.Models
{
    public class BugTracking
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public string Project { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string Submitter { get; set; }
    }
}

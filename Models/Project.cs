namespace OfficeManagement.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProjectUser { get; set; }
    }
}

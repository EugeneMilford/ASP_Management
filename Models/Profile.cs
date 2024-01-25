namespace OfficeManagement.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileDescription { get; set; }
        public string Experience { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public DateTime DateJoined { get; set; }
    }
}

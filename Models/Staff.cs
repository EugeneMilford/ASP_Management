namespace OfficeManagement.Models
{
    public class Staff
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime DateJoined { get; set; }
    }
}

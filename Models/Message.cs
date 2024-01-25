namespace OfficeManagement.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string MessageSender { get; set; }
        public string MessageBody { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

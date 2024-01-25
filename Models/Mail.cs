namespace OfficeManagement.Models
{
    public class Mail
    {
        public int MailId { get; set; }
        public string MailTopic { get; set; }
        public string MailContent { get; set; }
        public string Sender { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

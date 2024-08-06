using System;
using System.ComponentModel.DataAnnotations;

namespace OfficeManagement.Models
{
    public class Mail
    {
        public int MailId { get; set; }

        [Required]
        [Display(Name = "Topic")]
        public string MailTopic { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string MailContent { get; set; }

        [Required]
        [Display(Name = "Sender")]
        public string Sender { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }
    }
}

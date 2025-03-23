using OfficeManagement.Areas.Identity.Data;
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
        [Display(Name = "Is Spam")]
        public bool IsSpam { get; set; }
        public string UserId { get; set; }
        public OfficeUser User { get; set; }
    }
}
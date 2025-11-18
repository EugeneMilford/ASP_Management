using System;
using OfficeManagement.Models;
using Xunit;

namespace OfficeManagement.Tests.Pages.UserMail
{
    public class MailModelTests
    {
        [Fact]
        public void Mail_Defaults_IsSpamFalse()
        {
            var mail = new Mail
            {
                MailTopic = "T",
                MailContent = "C",
                Sender = "S",
                CreatedDate = DateTime.UtcNow
            };

            Assert.False(mail.IsSpam);
        }

        [Fact]
        public void Mail_RequiresFields_AreSet()
        {
            var created = DateTime.UtcNow;
            var mail = new Mail
            {
                MailTopic = "Topic",
                MailContent = "Content",
                Sender = "Sender",
                CreatedDate = created,
                UserId = "user1"
            };

            Assert.Equal("Topic", mail.MailTopic);
            Assert.Equal("Content", mail.MailContent);
            Assert.Equal("Sender", mail.Sender);
            Assert.Equal(created, mail.CreatedDate);
            Assert.Equal("user1", mail.UserId);
        }
    }
}

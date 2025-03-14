using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MimeKit;

namespace MauiEmail.Models
{
    public class ObservableMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public UniqueId UniqueId { get; set; }
        public DateTimeOffset Date {  get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public MimeKit.MailboxAddress From { get; set; }
        public List<MailboxAddress> To { get; set; }   
        public bool IsRead { get; set; }
        public bool IsFavourite { get; set; }

        public ObservableMessage(IMessageSummary message)
        {
            UniqueId = message.UniqueId;
            From = (MailboxAddress)message.Envelope.From[0];
            To = (List<MailboxAddress>)message.Envelope.To.Mailboxes.ToList();
            Subject = message.Envelope.Subject;
            Date = (DateTimeOffset)message.Envelope.Date;
            IsRead = (message.Flags & MessageFlags.Seen) == MessageFlags.Seen;
            IsFavourite = (message.Flags & MessageFlags.Flagged) == MessageFlags.Flagged;
            Body = null;
            HtmlBody = null;


        }

        public ObservableMessage(MimeMessage mimeMessage, UniqueId uniqueId) 
        { 
            UniqueId = uniqueId;
            From = (MailboxAddress)mimeMessage.From[0];
            To = (List<MailboxAddress>)mimeMessage.To.Mailboxes.ToList();
            Subject = mimeMessage.Subject;
            Date = mimeMessage.Date;
            Body = mimeMessage.TextBody;
            HtmlBody = mimeMessage.HtmlBody;
            IsRead = false;
            IsFavourite = false;
        }

        public MimeMessage ToMime()
        {
            return new MimeMessage(this);
        }

        public ObservableMessage GetForward()
        {
            var forwardMessage = new MimeMessage();

            forwardMessage.Subject = "FW: " + this.Subject;

            forwardMessage.From.Add(this.From); 

            foreach (var recipient in this.To)
            {
                forwardMessage.To.Add(recipient);  
            }

            forwardMessage.Date = DateTimeOffset.Now;

            var forwardBody = new StringBuilder();
            forwardBody.AppendLine("-------- Forwarded message --------");
            forwardBody.AppendLine("From: " + this.From.Address);
            forwardBody.AppendLine("To: " + string.Join(", ", this.To.Select(to => to.Address)));
            forwardBody.AppendLine();
            forwardBody.AppendLine(this.Body);

            forwardMessage.Body = new TextPart("plain") { Text = forwardBody.ToString() };

            return new ObservableMessage(forwardMessage, this.UniqueId);
        }



    }
}

using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiEmail.Interfaces
{
    public interface IMailConfig
    {
        string EmailAddress { get; set; }
        string Password { get; set; }

        string ReceiveHost { get; set; }
        SecureSocketOptions ReceiveSocketOptions { get; set; }
        int ReceivePort { get; set; }

        string SendHost { get; set; }
        int SendPort { get; set; }
        SecureSocketOptions SendSocketOptions { get; set; }

        string OAuth2ClientId { get; set; }
        string OAuth2ClientSecret { get; set; }
        string OAuth2RefreshToken { get; set; }
    }
}

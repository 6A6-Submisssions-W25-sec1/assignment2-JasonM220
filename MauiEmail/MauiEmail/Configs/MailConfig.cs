using MauiEmail.Interfaces;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiEmail.Configs
{
    internal class MailConfig : IMailConfig
    {
        public string EmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string ReceiveHost { get; set; } = string.Empty;
        public SecureSocketOptions ReceiveSocketOptions { get; set; } = SecureSocketOptions.Auto;
        public int ReceivePort { get; set; } = 993;

        public string SendHost { get; set; } = string.Empty;
        public int SendPort { get; set; } = 587;
        public SecureSocketOptions SendSocketOptions { get; set; } = SecureSocketOptions.StartTls;

        public string OAuth2ClientId { get; set; } = string.Empty;
        public string OAuth2ClientSecret { get; set; } = string.Empty;
        public string OAuth2RefreshToken { get; set; } = string.Empty;
    }
}

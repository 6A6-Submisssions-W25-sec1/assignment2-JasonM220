using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MauiEmail.Configs;
using MauiEmail.Services;

namespace MauiEmail
{
    public partial class App : Application
    {

        public static MailConfig EmailConfig { get; set; }
        public static EmailService EmailService { get; set; }
        public App()
        {
            InitializeComponent();

            EmailConfig = new MailConfig
            {
                EmailAddress = "jamauctest@gmail.com",
                Password = "tcyr cmnj hhtt hcnl",
                ReceiveHost = "imap.gmail.com",
                ReceiveSocketOptions = SecureSocketOptions.SslOnConnect,
                ReceivePort = 993,
                SendHost = "smtp.gmail.com",
                SendPort = 587,
                SendSocketOptions = SecureSocketOptions.StartTls
            };

            var imapClient = new ImapClient();
            var smtpClient = new SmtpClient();

            EmailService = new EmailService(EmailConfig, imapClient, smtpClient);

            MainPage = new AppShell();
        }
    }
}

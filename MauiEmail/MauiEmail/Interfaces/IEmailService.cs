using MauiEmail.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MailKit;

namespace MauiEmail.Interfaces
{
    public interface IEmailService
    {
        Task StartSendClientAsync();
        Task DisconnectSendClientAsync();
        Task StartRetreiveClientAsync();
        Task DisconnectRetreiveClientAsync();
        Task SendMessageAsync(MimeMessage message);
        Task<IEnumerable<ObservableMessage>?> FetchAllMessages();
        Task<IEnumerable<MimeMessage>> DownloadAllEmailsAsync();
        Task DeleteMessageAsync(MailKit.UniqueId uid);
    }
}

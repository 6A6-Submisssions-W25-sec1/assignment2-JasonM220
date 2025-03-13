using MauiEmail.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UniqueId = MailKit.UniqueId;
using MauiEmail.Models;
using MailKit.Search;

namespace MauiEmail.Services
{
    public class EmailService : IEmailService
    {
        private readonly IMailConfig _config;
        private IMailTransport _smtpClient;
        private IMailStore _imapClient;

        public EmailService(IMailConfig config, IMailStore receiveClient, IMailTransport sendClient)
        {
            _config = config;
            _smtpClient = sendClient;
            _imapClient = receiveClient;
        }

        public async Task StartSendClientAsync()
        {
            try
            {
                if (!_smtpClient.IsConnected)
                    await _smtpClient.ConnectAsync(_config.SendHost, _config.SendPort, _config.SendSocketOptions);

                if (!_smtpClient.IsAuthenticated)
                    await _smtpClient.AuthenticateAsync(_config.EmailAddress, _config.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting send client: {ex.Message}");
            }
        }

        public async Task DisconnectSendClientAsync()
        {
            if (_smtpClient.IsConnected)
                await _smtpClient.DisconnectAsync(true);
        }

        public async Task StartRetreiveClientAsync()
        {
            try
            {
                if (!_imapClient.IsConnected)
                    await _imapClient.ConnectAsync(_config.ReceiveHost, _config.ReceivePort, _config.ReceiveSocketOptions);

                if (!_imapClient.IsAuthenticated)
                    await _imapClient.AuthenticateAsync(_config.EmailAddress, _config.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting retrieve client: {ex.Message}");
            }
        }

        public async Task DisconnectRetreiveClientAsync()
        {
            if (_imapClient.IsConnected)
                await _imapClient.DisconnectAsync(true);
        }

        public async Task SendMessageAsync(MimeMessage message)
        {
            try
            {
                await _smtpClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public async Task<IEnumerable<MimeMessage>> DownloadAllEmailsAsync()
        {
            try
            {
                var inbox = _imapClient.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                var messages = new List<MimeMessage>();
                for (int i = 0; i < inbox.Count; i++)
                {
                    messages.Add(await inbox.GetMessageAsync(i));
                }

                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading emails: {ex.Message}");
                return new List<MimeMessage>();
            }
        }

        public async Task DeleteMessageAsync(UniqueId uid)
        {
            try
            {
                var inbox = _imapClient.Inbox;

                await inbox.OpenAsync(FolderAccess.ReadWrite);


                await inbox.StoreAsync(uid, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });


                await inbox.ExpungeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting email: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ObservableMessage>?> FetchAllMessages()
        {
            try
            {
                await this.StartSendClientAsync();
                await this.StartRetreiveClientAsync();

                var inbox = _imapClient.Inbox;

                await inbox.OpenAsync(FolderAccess.ReadOnly);


                var messages = new List<ObservableMessage>();
                foreach (var summary in await inbox.FetchAsync(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.UniqueId))
                {
                    messages.Add(new ObservableMessage(summary));
                }

                return messages;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching emails: {ex.Message}");
                return new List<ObservableMessage>();
            }
        }

        public async Task MarkAsReadAsync(UniqueId uid)
        {
            try
            {
                var inbox = _imapClient.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                await inbox.StoreAsync(uid, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Seen));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking email as read: {ex.Message}");
            }
        }

        public async Task MarkAsFavoriteAsync(UniqueId uid)
        {
            try
            {
                var inbox = _imapClient.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                await inbox.StoreAsync(uid, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Flagged));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking email as favorite: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UniqueId>> SearchEmailsAsync(string query)
        {
            try
            {
                var inbox = _imapClient.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                var searchQuery = SearchQuery.Or(
                    SearchQuery.SubjectContains(query),
                    SearchQuery.BodyContains(query)
                ).Or(SearchQuery.FromContains(query));

                var matchingMessages = await inbox.SearchAsync(searchQuery);

                return matchingMessages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error performing search query: {ex.Message}");
                return Enumerable.Empty<UniqueId>();
            }
        }

    }
}

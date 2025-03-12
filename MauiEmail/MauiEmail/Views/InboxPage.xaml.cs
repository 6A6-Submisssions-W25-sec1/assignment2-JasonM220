using MauiEmail.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiEmail.Views;

public partial class InboxPage : ContentPage
{
    public ObservableCollection<ObservableMessage> Emails { get; set; }

    private bool IsLoading = false;

    public InboxPage()
    {
        InitializeComponent();
        BindingContext = this;

        Emails = new ObservableCollection<ObservableMessage>();

        DownloadEmails();
    }

    private async void DownloadEmails()
    {
        var messages = await App.EmailService.FetchAllMessages();
        foreach (var msg in messages)
            Emails.Add(msg);
    }




    private async void OnEmailSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0) return;

        var selectedEmail = e.CurrentSelection.FirstOrDefault() as ObservableMessage;
        if (selectedEmail != null)
        {
            await App.EmailService.MarkAsReadAsync(selectedEmail.UniqueId);

            //await Navigation.PushAsync(new ReadPage(selectedEmail));
        }

        EmailList.SelectedItem = null;
    }
}

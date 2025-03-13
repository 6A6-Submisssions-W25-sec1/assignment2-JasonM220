using MauiEmail.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiEmail.Views;

public partial class InboxPage : ContentPage
{
    public ObservableCollection<ObservableMessage> Emails { get; set; }

    private bool IsLoading = false;
    public ICommand DeleteCommand { private set; get; }
    public ICommand FavouriteCommand { private set; get; }

    public InboxPage()
    {
        InitializeComponent();
        BindingContext = this;

        Emails = new ObservableCollection<ObservableMessage>();
        DeleteCommand = new Command<ObservableMessage>(DeleteEmail);
        FavouriteCommand = new Command<ObservableMessage>(FavouriteEmail);


        DownloadEmails();
    }

    private async void DownloadEmails()
    {
        var messages = await App.EmailService.FetchAllMessages();
        foreach (var msg in messages)
            Emails.Add(msg);
    }

    private async void DeleteEmail(ObservableMessage email)
    {
        Emails.Remove(email);
        await App.EmailService.DeleteMessageAsync(email.UniqueId);
    }  
    private async void FavouriteEmail(ObservableMessage email)
    {
        email.IsFavourite = true;
        await App.EmailService.MarkAsFavoriteAsync(email.UniqueId);
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

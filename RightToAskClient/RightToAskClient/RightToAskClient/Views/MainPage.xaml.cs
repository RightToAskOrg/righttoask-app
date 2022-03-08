using System;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            // If we don't have a server public key, then there was something wrong with our initialization file.
            // Although this is not a useful message for users, it's a sufficiently useful message for developers
            // that it's worth producing here.
            if (String.IsNullOrEmpty(RTAClient.ServerPublicKey))
            {
                DisplayAlert(AppResources.ServerConfigErrorText, AppResources.ServerConfigErrorRecommendation, "", "OK");
            }
        }

        async void OnTop10NowButtonClicked(object sender, EventArgs e)
        {
            App.ReadingContext.TopTen = true;
            await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
        }

        private void OnExpiringSoonButtonClicked(object sender, EventArgs e)
        {
            OnTop10NowButtonClicked(sender, e);
        }

        // If either 'enter' is pressed after a keyword change, or the 
        // 'search by keyword' button is pressed, launch the reading page.
        // Otherwise, if only the keyword is changed, update it but don't
        // launch a new page.
        void OnReadByKeywordFieldCompleted(object sender, EventArgs e)
        {
            App.ReadingContext.Filters.SearchKeyword = ((SearchBar)sender).Text;
            LaunchKeywordReadingPage();
        }

        private void OnKeywordChanged(object sender, TextChangedEventArgs e)
        {
            App.ReadingContext.Filters.SearchKeyword = e.NewTextValue;
        }

        async void LaunchKeywordReadingPage()
        {
            App.ReadingContext.IsReadingOnly = true;
            //await Navigation.PushAsync(readingPage);
            await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
        }

        async void OnNavigateButtonClicked(object sender, EventArgs e)
        {
            App.ReadingContext.IsReadingOnly = false;
            //await Navigation.PushAsync (secondPage);
            await Shell.Current.GoToAsync($"{nameof(SecondPage)}");
        }

        async void OnReadButtonClicked(object sender, EventArgs e)
        {
            App.ReadingContext.IsReadingOnly = true;
            //await Navigation.PushAsync(secondPage);
            await Shell.Current.GoToAsync($"{nameof(SecondPage)}");
        }
    }
}

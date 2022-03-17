using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        private static string appDomainString = "https://righttoask.democracydevelopers.org.au/";
        private static Uri appDomain = new Uri(appDomainString);
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            GoBack();
            return true;
        }

        public void OnBackButtonClicked(object sender, EventArgs e)
        {
            GoBack();
        }
        private async void GoBack()
        {
            if (webView.CanGoBack)
            {
                webView.GoBack();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        public void OnForwardButtonClicked(object sender, EventArgs e)
        {
            if (webView.CanGoForward)
            {
                webView.GoForward();
            }
        }

        public async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith(appDomainString))
            {
                // save the destination url
                var destination = e.Url;

                // cancel the navigation
                e.Cancel = true;

                // display an alert
                string alertTitle = "You are trying to navigate to: " + destination + " which is not part of our Website. " +
                    "Please copy the link and paste it into your web browser if you wish to continue.";
                string alertCancel = "Cancel";
                string alertDestruction = "Proceed";
                string[] alertButtons = { "Copy Link" };
                //string? result = await Shell.Current.DisplayActionSheet("You are trying to navigate to: " + destination + " which is not part of our Website. " +
                //    "Please copy the link and paste it into your web browser if you wish to continue.", "Go Back", "Copy Link", "Proceed");
                string? result = await Shell.Current.DisplayActionSheet(alertTitle, alertCancel, alertDestruction, alertButtons);
                if (result == "Copy Link")
                {
                    await Clipboard.SetTextAsync(destination);
                }
                else if(result == "Proceed")
                {
                    Uri browserDestination = new Uri(destination);
                    await OpenBrowser(browserDestination);
                }
            }
        }

        public async Task OpenBrowser(Uri uri)
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
                Debug.WriteLine("Error occured Opening Browser");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
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
            var vm = (BaseViewModel)BindingContext;
            vm.PopupLabelText = AppResources.AboutPagePopupText;
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
                string alertTitle = AppResources.WebNavigationWarning +"\n" + destination;
                string alertCancel = AppResources.CancelButtonText;
                string alertDestruction = AppResources.NavigateOKText; 
                //string[] alertButtons = { "Copy Link" };
                //string? result = await Shell.Current.DisplayActionSheet(alertTitle, alertCancel, alertDestruction, alertButtons);
                string? result = await Shell.Current.DisplayActionSheet(alertTitle, alertCancel, alertDestruction);
                //if (result == "Copy Link")
                //{
                //    await Clipboard.SetTextAsync(destination);
                //}
                if(result == AppResources.NavigateOKText)
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
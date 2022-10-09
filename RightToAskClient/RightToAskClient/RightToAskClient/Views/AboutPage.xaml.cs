using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
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
        private BaseViewModel baseViewModel;
        public AboutPage()
        {
            InitializeComponent();
            baseViewModel = (BaseViewModel)BindingContext;
            baseViewModel.PopupLabelText = AppResources.AboutPagePopupText;
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
                var alertText = AppResources.WebNavigationWarning + "\n" + destination;
                var alertCancel = AppResources.CancelButtonText;
                var alertConfirmation = AppResources.NavigateOKText;

                var popup = new TwoButtonPopup(baseViewModel, AppResources.NavigationPopupTitle, alertText, alertCancel, alertConfirmation);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (baseViewModel.ApproveButtonClicked)
                {
                    var browserDestination = new Uri(destination);
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
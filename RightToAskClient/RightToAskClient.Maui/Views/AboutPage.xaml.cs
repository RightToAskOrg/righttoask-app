using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.Maui.Resx;
using RightToAskClient.Maui.ViewModels;
using RightToAskClient.Maui.Views.Popups;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        private readonly BaseViewModel baseViewModel;
        public AboutPage()
        {
            InitializeComponent();
            baseViewModel = (BaseViewModel)BindingContext;
            baseViewModel.PopupLabelText = AppResources.AboutPagePopupText;
            webView.Source = Constants.DDHowItWorksURL;
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
            if (!e.Url.StartsWith(Constants.DDBaseURL))
            {
                // save the destination url
                var destination = e.Url;

                // cancel the navigation
                e.Cancel = true;

                // display an alert
                var alertText = AppResources.WebNavigationWarning + "\n" + destination;
                var alertCancel = AppResources.CancelButtonText;
                var alertConfirmation = AppResources.NavigateOKText;

              //  var popup = new TwoButtonPopup(AppResources.NavigationPopupTitle, alertText, alertCancel, alertConfirmation, false);
                var popupResult = await Application.Current.MainPage.DisplayPromptAsync(AppResources.NavigationPopupTitle, alertText, alertConfirmation, alertCancel);
                if (popupResult == alertConfirmation)
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
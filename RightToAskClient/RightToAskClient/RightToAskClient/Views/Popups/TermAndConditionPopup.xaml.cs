using System;
using RightToAskClient.Helpers;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermAndConditionPopup : Popup
    {
        public TermAndConditionPopup()
        {
            InitializeComponent();
        }
        
        private void okButton_Clicked(object sender, EventArgs e)
        {
            XamarinPreferences.shared.Set(Constants.ShowFirstTimeReadingPopup, false);
            Dismiss("Dismissed");
        }

        private async void PrivacyPolicy_OnTapped(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://righttoask.democracydevelopers.org.au/privacy-policy/", BrowserLaunchMode.SystemPreferred);
        }
        
        private async void TermAndCondition_OnTapped(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://righttoask.democracydevelopers.org.au/", BrowserLaunchMode.SystemPreferred);
        }
    }
}
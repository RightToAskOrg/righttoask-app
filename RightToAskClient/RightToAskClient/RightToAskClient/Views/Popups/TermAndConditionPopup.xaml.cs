using System;
using RightToAskClient.Helpers;
using RightToAskClient.Resx;
using Xam.Forms.Markdown;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermAndConditionPopup : Popup
    {
        public TermAndConditionPopup()
        {
            InitializeComponent();

            var lightTheme = new LightMarkdownTheme();
            lightTheme.Paragraph.FontSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Label));
            lightTheme.BackgroundColor = Color.Transparent;
            lightTheme.Link.ForegroundColor = (Color)Application.Current.Resources["UrlTextColorLightMode"];

            var darkTheme = new DarkMarkdownTheme();
            darkTheme.Paragraph.FontSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Label));
            darkTheme.BackgroundColor = Color.Transparent;
            darkTheme.Link.ForegroundColor = (Color)Application.Current.Resources["UrlTextColorDarkMode"];

            var mdView = new MarkdownView();

            mdView.Markdown = AppResources.TermsAndConditionsMarkdown;
            mdView.RelativeUrlHost = "";
            AutomationProperties.SetIsInAccessibleTree(mdView, true);
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                AutomationProperties.SetHelpText(mdView, AppResources.TermsAndConditionsHelpTextAndroid);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                AutomationProperties.SetHelpText(mdView, AppResources.TermsAndConditionsHelpTextIOS);
            }

            mdView.SetOnAppTheme<MarkdownTheme>(Xam.Forms.Markdown.MarkdownView.ThemeProperty, lightTheme, darkTheme);

            MarkdownView.Children.Add(new ScrollView() { Content = mdView });
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            XamarinPreferences.shared.Set(Constants.ShowFirstTimeReadingPopup, false);
            Dismiss("Dismissed");
        }

        /*
        private async void PrivacyPolicy_OnTapped(object sender, EventArgs e)
        {
            await Browser.OpenAsync(Constants.DDPrivacyPolicyURL,
                BrowserLaunchMode.SystemPreferred);
        }

        private async void TermAndCondition_OnTapped(object sender, EventArgs e)
        {
            await Browser.OpenAsync(Constants.DDTermsAndConditionsURL,
                BrowserLaunchMode.SystemPreferred);
        }
        */
    }
}
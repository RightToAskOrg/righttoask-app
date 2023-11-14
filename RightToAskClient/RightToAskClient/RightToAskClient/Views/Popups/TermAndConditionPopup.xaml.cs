using System;
using RightToAskClient.Helpers;
using RightToAskClient.Resx;
using Xam.Forms.Markdown;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.ImageSources;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Maui.Views;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermAndConditionPopup : Popup
    {
        public TermAndConditionPopup()
        {
            InitializeComponent();
            //TODO:
            //var lightTheme = new LightMarkdownTheme();
            //// TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            //lightTheme.Paragraph.FontSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Label));
            //lightTheme.BackgroundColor = Colors.Transparent;
            //lightTheme.Link.ForegroundColor = (Color)Application.Current.Resources["UrlTextColorLightMode"];

            //var darkTheme = new DarkMarkdownTheme();
            //// TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            //darkTheme.Paragraph.FontSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Label));
            //darkTheme.BackgroundColor = Colors.Transparent;
            //darkTheme.Link.ForegroundColor = (Color)Application.Current.Resources["UrlTextColorDarkMode"];

            //var mdView = new MarkdownView();

            //mdView.Markdown = AppResources.TermsAndConditionsMarkdown;
            //mdView.RelativeUrlHost = "";
            //AutomationProperties.SetIsInAccessibleTree(mdView, true);
            //if (DeviceInfo.Platform == DevicePlatform.Android)
            //{
            //    AutomationProperties.SetHelpText(mdView, AppResources.TermsAndConditionsHelpTextAndroid);
            //}
            //else if (DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    AutomationProperties.SetHelpText(mdView, AppResources.TermsAndConditionsHelpTextIOS);
            //}

            //mdView.SetOnAppTheme<MarkdownTheme>(Xam.Forms.Markdown.MarkdownView.ThemeProperty, lightTheme, darkTheme);

            //MarkdownView.Children.Add(new ScrollView() { Content = mdView });
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            XamarinPreferences.shared.Set(Constants.ShowFirstTimeReadingPopup, false);
            //TODO: Dismiss("Dismissed");
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
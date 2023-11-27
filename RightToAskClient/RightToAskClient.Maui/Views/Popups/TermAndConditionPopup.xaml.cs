using System;
using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.Resx;
////using Xam.Forms.Markdown;
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

namespace RightToAskClient.Maui.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermAndConditionPopup : Popup
    {
        //TODO: localisation of the terms text will need to be handled at a later point when localisation occurs as the hyperlinks are seperate
        //and this was left as the MAUI conversion was a lengthy process
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
            //if (DeviceInfo.Platform == DevicePlatform.Android)
            //{
            //    TermsText.Text = AppResources.TermsAndConditionsHelpTextAndroid;
            //}
            //else
            //{
            //    TermsText.Text = AppResources.TermsAndConditionsHelpTextIOS;
            //}
            //mdView.SetOnAppTheme<MarkdownTheme>(Xam.Forms.Markdown.MarkdownView.ThemeProperty, lightTheme, darkTheme);

            //MarkdownView.Children.Add(new ScrollView() { Content = mdView });
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            XamarinPreferences.shared.Set(Constants.ShowFirstTimeReadingPopup, false);
            Close();
        }


        //private async void PrivacyPolicy_OnTapped(object sender, EventArgs e)
        //{
        //    await Browser.OpenAsync(Constants.DDPrivacyPolicyURL,
        //        BrowserLaunchMode.SystemPreferred);
        //}

        //private async void TermAndCondition_OnTapped(object sender, EventArgs e)
        //{
        //    await Browser.OpenAsync(Constants.DDTermsAndConditionsURL,
        //        BrowserLaunchMode.SystemPreferred);
        //}

    }
}
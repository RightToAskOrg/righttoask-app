using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Views
{
    public partial class CodeOfConductPage : ContentPage
    {
        private Registration _registration;
        
        public CodeOfConductPage()
        {
            InitializeComponent();
        }
        
        public CodeOfConductPage(Registration registration)
        {
            InitializeComponent();
            _registration = registration;
            //TODO
            /*
            var lightTheme = new LightMarkdownTheme();
            var darkTheme = new DarkMarkdownTheme();
            // TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            lightTheme.Paragraph.FontSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Label));
            // TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            lightTheme.Heading3.FontSize = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            // TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            darkTheme.Paragraph.FontSize = (float)Device.GetNamedSize(NamedSize.Small, typeof(Label));
            // TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            darkTheme.Heading3.FontSize = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            darkTheme.BackgroundColor = Colors.Black;
            */
            var mdView = new Xam.Forms.Markdown.MarkdownView();
           
            mdView.Markdown = AppResources.CodeOfConductCopy;
            mdView.RelativeUrlHost = "";
          //TODO:  mdView.SetOnAppTheme<MarkdownTheme>(Xam.Forms.Markdown.MarkdownView.ThemeProperty, lightTheme, darkTheme);
            
           //TODO: MarkdownView.Children.Add(new ScrollView() { Content = mdView });
        }

        private async void Disagree_OnClicked(object sender, EventArgs e)
        {
            //var popup = new TwoButtonPopup(
            //    "", 
            //    AppResources.DisagreeCodeOfConductPopText, 
            //    AppResources.CancelButtonText, 
            //    AppResources.OKText,  
            //    false);
            var popupResult = await App.Current.MainPage.DisplayPromptAsync("", AppResources.DisagreeCodeOfConductPopText, AppResources.OKText,
                AppResources.CancelButtonText);
            //var popupResult = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (popupResult == AppResources.OKText)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }
        
        private async void Agree_OnClicked(object sender, EventArgs e)
        {
            var registerAccountFlow = new RegisterAccountPage(_registration);
            await Application.Current.MainPage.Navigation.PushAsync(registerAccountFlow);
        }
        
    }
}
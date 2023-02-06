using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using Xam.Forms.Markdown;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

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

            var lightTheme = new LightMarkdownTheme();
            var darkTheme = new DarkMarkdownTheme();
            lightTheme.Paragraph.FontSize = 15;
            lightTheme.Heading3.FontSize = 18;
            darkTheme.Paragraph.FontSize = 15;
            darkTheme.Heading3.FontSize = 18;
            darkTheme.BackgroundColor = Color.Black;

            var mdView = new Xam.Forms.Markdown.MarkdownView();
           
            mdView.Markdown = AppResources.CodeOfConductCopy;
            mdView.RelativeUrlHost = "";
            mdView.SetOnAppTheme<MarkdownTheme>(Xam.Forms.Markdown.MarkdownView.ThemeProperty, lightTheme, darkTheme);
            
            MarkdownView.Children.Add(new ScrollView() { Content = mdView });
        }

        private async void Disagree_OnClicked(object sender, EventArgs e)
        {
            var popup = new TwoButtonPopup(
                "", 
                AppResources.DisagreeCodeOfConductPopText, 
                AppResources.CancelButtonText, 
                AppResources.OKText, 
                false);
            var popupResult = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            if (popup.HasApproved(popupResult))
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
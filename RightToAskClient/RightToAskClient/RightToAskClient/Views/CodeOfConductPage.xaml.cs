using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
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
            
            var mdView = new Xam.Forms.Markdown.MarkdownView();
            mdView.Theme.Paragraph.FontSize = 15;
            mdView.Markdown = AppResources.CodeOfConductCopy;
            mdView.RelativeUrlHost = "";
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
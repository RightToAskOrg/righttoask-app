using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
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
            mdView.Markdown = AppResources.CodeOfConductCopy;
            mdView.RelativeUrlHost = "";
            MarkdownView.Children.Add(new ScrollView() { Content = mdView });
        }

        private void Disagree_OnClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
        
        private void Agree_OnClicked(object sender, EventArgs e)
        {
            var registerAccountFlow = new RegisterAccountPage(_registration);
            Application.Current.MainPage.Navigation.PushAsync(registerAccountFlow);
        }
        
    }
}
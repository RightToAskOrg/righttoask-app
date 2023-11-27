using System;
using RightToAskClient.Maui.Resx;
//using Xam.Forms.Markdown;
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
    public partial class OneButtonPopup : ContentPage
    {
        public OneButtonPopup()
        {

        }
        public OneButtonPopup(string message, string buttonText)
        {
            initialize("",message, buttonText, false);
        }

        public OneButtonPopup(string message, string buttonText, bool isInfoPopup)
        {
            initialize("",message, buttonText, true);
        }  

        public OneButtonPopup(string title, string message, string buttonText)
        {
            initialize(title, message, buttonText, false);
        }
        
        public OneButtonPopup(string title, string message, string buttonText, bool isInfoPopup)
        {
            initialize(title, message, buttonText, true);
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PopModalAsync();
        }

        private void initialize(string title, string message, string buttonText, bool isInfoPopup)
        {
            InitializeComponent();
            mainTitle.IsVisible = title.Length > 0;
            mainTitle.Text = title;
            mainMessage.Text = message;
            okButton.Text = buttonText;
            EmuImage.IsVisible = isInfoPopup;
            //TODO:
           // if (isInfoPopup)
              //  EmuAndButtonLayout.SetAppThemeColor(BackgroundColorProperty, Colors.White, (Color) Application.Current.Resources["BackgroundDarkGray"]);
        }
    }
}
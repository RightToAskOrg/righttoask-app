using System;
using RightToAskClient.Resx;
using Xam.Forms.Markdown;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OneButtonPopup : Popup
    {
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
            Dismiss("Dismissed");
        }

        private void initialize(string title, string message, string buttonText, bool isInfoPopup)
        {
            InitializeComponent();
            mainTitle.IsVisible = title.Length > 0;
            mainTitle.Text = title;
            mainMessage.Text = message;
            okButton.Text = buttonText;
            EmuImage.IsVisible = isInfoPopup;
            if(isInfoPopup)
                EmuAndButtonLayout.SetAppThemeColor(BackgroundColorProperty, Color.White, (Color) Application.Current.Resources["PopupDarkModeBgColor"]);
        }
    }
}
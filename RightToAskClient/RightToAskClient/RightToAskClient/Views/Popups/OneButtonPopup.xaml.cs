using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OneButtonPopup : Popup
    {
        public OneButtonPopup(string message, string buttonText)
        {
            initialize("",message, buttonText, false);
            // InitializeComponent();
            // mainTitle.IsVisible = false;
            // mainMessage.Text = message;
            // okButton.Text = buttonText;
        }

        public OneButtonPopup(string message, string buttonText, bool isInfoPopup)
        {
            initialize("",message, buttonText, true);
        }  

        public OneButtonPopup(string title, string message, string buttonText)
        {
            initialize(title, message, buttonText, false);
            // InitializeComponent();
            // mainTitle.IsVisible = true;
            // mainTitle.Text = title;
            // mainMessage.Text = message;
            // okButton.Text = buttonText;
        }
        
        public OneButtonPopup(string title, string message, string buttonText, bool isInfoPopup)
        {
            initialize(title, message, buttonText, true);
            // InitializeComponent();
            // mainTitle.IsVisible = true;
            // mainTitle.Text = title;
            // mainMessage.Text = message;
            // okButton.Text = buttonText;
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
            separatorView.IsVisible = !isInfoPopup;
            EmuImage.IsVisible = isInfoPopup;
        }
    }
}
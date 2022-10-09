using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OneButtonPopup : Popup
    {
        public OneButtonPopup(string message, string buttonText)
        {
            InitializeComponent();
            mainTitle.IsVisible = false;
            mainMessage.Text = message;
            okButton.Text = buttonText;
        }

        public OneButtonPopup(string title, string message, string buttonText)
        {
            InitializeComponent();
            mainTitle.IsVisible = true;
            mainTitle.Text = title;
            mainMessage.Text = message;
            okButton.Text = buttonText;
        }

        private void okButton_Clicked(object sender, EventArgs e)
        {
            Dismiss("Dismissed");
        }
    }
}
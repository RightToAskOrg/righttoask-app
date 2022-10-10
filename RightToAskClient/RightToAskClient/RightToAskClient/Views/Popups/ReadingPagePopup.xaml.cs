using System;
using RightToAskClient.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadingPagePopup : Popup
    {
        public ReadingPagePopup(ReadingPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Dismiss("Dismissed");
        }
    }
}
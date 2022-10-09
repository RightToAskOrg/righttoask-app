using RightToAskClient.ViewModels;
using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
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
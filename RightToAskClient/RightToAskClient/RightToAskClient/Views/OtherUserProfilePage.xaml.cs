using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherUserProfilePage : ContentPage
    {
        public OtherUserProfilePage(Registration registration)
        {
            InitializeComponent();
            BindingContext = new RegistrationViewModel(registration); 
        }
    }
}
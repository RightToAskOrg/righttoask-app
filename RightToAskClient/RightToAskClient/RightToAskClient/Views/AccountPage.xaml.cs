using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage(Registration user, RegistrationState registrationState)
        {
            InitializeComponent();

            BindingContext = new RegistrationViewModel(user, registrationState);
            var reg = BindingContext as RegistrationViewModel;
            reg?.ReinitRegistrationUpdates();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
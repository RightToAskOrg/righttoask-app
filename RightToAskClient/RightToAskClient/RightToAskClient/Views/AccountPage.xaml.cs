using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
            
            var reg = BindingContext as RegistrationViewModel;
            reg?.ReinitRegistrationUpdates();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
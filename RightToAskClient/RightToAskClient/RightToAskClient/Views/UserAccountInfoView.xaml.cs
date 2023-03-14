using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAccountInfoView : ContentView
    {
        private RegistrationViewModel? vm;
        public UserAccountInfoView()
        {
            InitializeComponent();
        }
    }
}
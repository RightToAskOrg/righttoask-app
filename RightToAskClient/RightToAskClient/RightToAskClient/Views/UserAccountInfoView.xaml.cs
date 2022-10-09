using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAccountInfoView : ContentView
    {
        public UserAccountInfoView()
        {
            InitializeComponent();

            var caneditUID = (BindingContext as RegistrationViewModel)?.CanEditUID ?? false;
            UIDEntry.Style = caneditUID
                ? App.Current.Resources["NormalEntry"] as Style
                : App.Current.Resources["DisabledEntry"] as Style;
        }
    }
}
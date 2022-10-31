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

            var caneditUID = (BindingContext as RegistrationViewModel)?.CanEditUid ?? false;
            UIDEntry.Style = caneditUID
                ? Application.Current.Resources["NormalEntry"] as Style
                : Application.Current.Resources["DisabledEntry"] as Style;
        }
    }
}
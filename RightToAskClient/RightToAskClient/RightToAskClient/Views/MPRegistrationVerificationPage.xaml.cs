using RightToAskClient.Helpers;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MPRegistrationVerificationPage : ContentPage
    {

        // Shell navigation requires a default constructor
        public MPRegistrationVerificationPage()
        {
            InitializeComponent();

            DomainPicker.ItemsSource = ParliamentaryURICreator.ValidParliamentaryDomains;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            
            var reg = BindingContext as Registration1ViewModel;
            reg.ReinitRegistrationUpdates();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
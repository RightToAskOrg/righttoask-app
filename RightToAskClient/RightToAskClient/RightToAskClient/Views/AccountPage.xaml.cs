using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        protected override bool OnBackButtonPressed()
        {
            //App.Current.MainPage.Navigation.PopToRootAsync();
            //_ = Shell.Current.GoToAsync(nameof(MainPage));
            Task<string>? result = Shell.Current.DisplayActionSheet("Are you sure you want to exit?", "Cancel", "Yes");
            if (result.ToString() == "Yes")
            {
                return false; // close the application
            }
            return true; // otherwise do nothing
        }
    }
}
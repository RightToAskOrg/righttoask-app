using System;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{

    public static class AccountPageExchanger
    {
        public static Registration? Registration;
    }
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (AccountPageExchanger.Registration != null)
            {
                BindingContext = new RegistrationViewModel(AccountPageExchanger.Registration);
                AccountPageExchanger.Registration = null;
            }
            var reg = BindingContext as RegistrationViewModel;
            // reg?.ReinitRegistrationUpdates();
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
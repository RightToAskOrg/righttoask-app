using System;
using RightToAskClient.Helpers;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

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
            IndividualParticipant.getInstance().Init();
            AccountPageExchanger.Registration = IndividualParticipant.getInstance().ProfileData.RegistrationInfo;
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
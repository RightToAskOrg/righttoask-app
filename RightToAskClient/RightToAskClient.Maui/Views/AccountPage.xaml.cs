using System;
using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.Models;
using RightToAskClient.Maui.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
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
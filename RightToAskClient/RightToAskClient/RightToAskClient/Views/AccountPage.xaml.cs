﻿using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{

    public static class AccountPageExchanger
    {
        public static Registration? Registration;
        public static RegistrationStatus? RegistrationStatus;
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
            if (AccountPageExchanger.Registration != null &&
                AccountPageExchanger.RegistrationStatus != null)
            {
                BindingContext = new RegistrationViewModel(
                    AccountPageExchanger.Registration,
                    AccountPageExchanger.RegistrationStatus ?? RegistrationStatus.NotRegistered);
                var reg = BindingContext as RegistrationViewModel;
                reg?.ReinitRegistrationUpdates();
            }
            
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
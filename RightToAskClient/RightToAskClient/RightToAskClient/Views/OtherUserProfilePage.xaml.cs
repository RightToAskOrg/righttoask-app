﻿using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherUserProfilePage : ContentPage
    {
        public OtherUserProfilePage(Registration user, RegistrationStatus registrationStatus)
        {
            InitializeComponent();
            BindingContext = new RegistrationViewModel(user, registrationStatus); 
        }
    }
}
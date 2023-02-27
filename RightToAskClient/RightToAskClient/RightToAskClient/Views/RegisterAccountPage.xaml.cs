using System;
using System.ComponentModel;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using RightToAskClient.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterAccountPage : ContentPage
    {
        public RegisterAccountPage(Registration registration)
        {
            InitializeComponent();
            BindingContext = new RegistrationViewModel(registration);
            var reg = BindingContext as RegistrationViewModel;
            reg.ReinitRegistrationUpdates();
        }

        /*
        void OnStatePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;

            if (picker.SelectedIndex != -1)
            {
                string state = (string)picker.SelectedItem;
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.SelectedStateAsIndex = picker.SelectedIndex;
                IndividualParticipant.getInstance().UpdateChambers(state);
            }
        }
        */

        private void OnRegisterEmailFieldCompleted(object sender, EventArgs e)
        {
            var viewModel = BindingContext as RegistrationViewModel;
            if (viewModel != null)
            {
                viewModel.SetUserEmail(((Editor)sender).Text);
            }
        }

        private void UIDEntry_OnUnfocused(object sender, FocusEventArgs e)
        {
            UsernameLabel.Style = Application.Current.Resources["PickerTitle"] as Style;
            UsernameEntryBorder.Style = Application.Current.Resources["EntryBorder"] as Style;
            var viewModel = BindingContext as RegistrationViewModel;
            viewModel?.ValidateUsername();
        }

        private void NameEntry_OnUnfocused(object sender, FocusEventArgs e)
        {
            NameLabel.Style = Application.Current.Resources["PickerTitle"] as Style;
            NameEntryBorder.Style = Application.Current.Resources["EntryBorder"] as Style;
            var viewModel = BindingContext as RegistrationViewModel;
            viewModel?.ValidateName();
        }

        private void NameEntry_OnFocused(object sender, FocusEventArgs e)
        {
            NameLabel.Style = Application.Current.Resources["PickerTitleSelected"] as Style;
            NameEntryBorder.Style = Application.Current.Resources["EntryBorderSelected"] as Style;
        }

        private void UIDEntry_OnFocused(object sender, FocusEventArgs e)
        {
            UsernameLabel.Style = Application.Current.Resources["PickerTitleSelected"] as Style;
            UsernameEntryBorder.Style = Application.Current.Resources["EntryBorderSelected"] as Style;
        }
    }
}
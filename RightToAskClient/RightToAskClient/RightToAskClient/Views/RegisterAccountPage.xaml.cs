using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
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
            UsernameEntry.Style = Application.Current.Resources["PickerTitle"] as Style;
            var viewModel = BindingContext as RegistrationViewModel;
            if (viewModel != null)
            {
                viewModel.ValidateUsername();
            }
        }

        private void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
        {
            NameEntry.Style = Application.Current.Resources["PickerTitle"] as Style;
            var viewModel = BindingContext as RegistrationViewModel;
            if (viewModel != null)
            {
                viewModel.ValidateName();
            }
        }

        private void VisualElement_OnFocused(object sender, FocusEventArgs e)
        {
            NameEntry.Style = Application.Current.Resources["PickerTitleSelected"] as Style;
        }

        private void UIDEntry_OnFocused(object sender, FocusEventArgs e)
        {
            UsernameEntry.Style = Application.Current.Resources["PickerTitleSelected"] as Style;
        }
    }
}
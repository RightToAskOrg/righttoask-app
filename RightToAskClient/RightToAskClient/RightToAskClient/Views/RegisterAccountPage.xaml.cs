using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterAccountPage : ContentPage
    {
        public RegisterAccountPage()
        {
            InitializeComponent();

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
    }
}
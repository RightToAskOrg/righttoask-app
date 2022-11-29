using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage1 : ContentPage
    {
        public RegisterPage1()
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
                IndividualParticipant.ProfileData.RegistrationInfo.SelectedStateAsIndex = picker.SelectedIndex;
                IndividualParticipant.UpdateChambers(state);
            }
        }
        */

        private void OnRegisterEmailFieldCompleted(object sender, EventArgs e)
        {
            IndividualParticipant.ProfileData.UserEmail = ((Editor)sender).Text;
        }
    }
}
using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowAnsweredOptionPage : ContentPage
    {
        public HowAnsweredOptionPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            RadioButtonChecked(HowAnsweredOptions.InApp, InAppRadioButton);
        }

        private void InApp_OnTapped(object sender, EventArgs e)
        {
            // InAppRadioButton.IsChecked = true;
            // var vm = BindingContext as QuestionViewModel;
            // vm.HowAnswered = HowAnsweredOptions.InApp;
            RadioButtonChecked(HowAnsweredOptions.InApp, InAppRadioButton);
            
        }
        
        private void InParliament_OnTapped(object sender, EventArgs e)
        {
            // InParliamentRadioButton.IsChecked = true;
            // var vm = BindingContext as QuestionViewModel;
            // vm.HowAnswered = HowAnsweredOptions.InParliament;
            RadioButtonChecked(HowAnsweredOptions.InParliament, InParliamentRadioButton);
        }
        
        private void DontKnow_OnTapped(object sender, EventArgs e)
        {
            // DontKnowRadioButton.IsChecked = true;
            // var vm = BindingContext as QuestionViewModel;
            // vm.HowAnswered = HowAnsweredOptions.DontKnow;
            RadioButtonChecked(HowAnsweredOptions.DontKnow, DontKnowRadioButton);
        }

        private void RadioButtonChecked(HowAnsweredOptions option, RadioButton radioButton)
        {
            radioButton.IsChecked = true;
            var vm = BindingContext as QuestionViewModel;
            vm.HowAnswered = option;
        }
    }
}
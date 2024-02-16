using System;
using RightToAskClient.Maui.Resx;
using RightToAskClient.Maui.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        public QuestionAskerPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            QuestionViewModel.Instance.PopupLabelText = AppResources.QuestionAskerPopupText;
            CommitteeRadioButton.IsChecked = true;
        }

        private void CommitteeRadioButton_OnTapped(object sender, EventArgs e)
        {
            CommitteeRadioButton.IsChecked = true;
        }
        
        private void MyMpRadioButton_OnTapped(object sender, EventArgs e)
        {
            MyMpRadioButton.IsChecked = true;
        }
        
        private void OtherMpRadioButton_OnTapped(object sender, EventArgs e)
        {
            OtherMpRadioButton.IsChecked = true;
        }
        
        private void DontKnowRadioButton_OnTapped(object sender, EventArgs e)
        {
            DontKnowRadioButton.IsChecked = true;
        }
        
        private void NextButton_OnClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as QuestionViewModel;
            if (CommitteeRadioButton.IsChecked)
                vm.FindCommitteeCommand.Execute(true);
            if (MyMpRadioButton.IsChecked)
                vm.myMPRaiseCommand.Execute(true);
            if (OtherMpRadioButton.IsChecked)
                vm.OtherMPRaiseCommand.Execute(true);
            if (DontKnowRadioButton.IsChecked)
                vm.NotSureWhoShouldRaiseCommand.Execute(true);
        }
    }
}
using System;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        public QuestionAskerPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            QuestionViewModel.Instance.PopupLabelText = AppResources.QuestionAskerPopupText;
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
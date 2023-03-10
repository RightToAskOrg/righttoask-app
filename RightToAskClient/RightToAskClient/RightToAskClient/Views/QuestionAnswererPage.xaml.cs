using System;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAnswererPage : ContentPage
    {
        public QuestionAnswererPage()
        {
            InitializeComponent();

            BindingContext = QuestionViewModel.Instance;
            QuestionViewModel.Instance.PopupLabelText = AppResources.AnswerQuestionOptionsPopupText;

            MyMpRadioButton.IsChecked = true;
        }

        private void NextButton_OnClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as QuestionViewModel;
            if (MyMpRadioButton.IsChecked)
                vm.AnsweredByMyMPCommand.ExecuteAsync();
            if (AnotherMpRadioButton.IsChecked)
                vm.AnsweredByOtherMPCommandOptionB.ExecuteAsync();
            if (PublicAuthorityRadioButton.IsChecked)
                vm.OtherPublicAuthorityButtonCommand.ExecuteAsync();
            if (DontKnowRadioButton.IsChecked)
                vm.LeaveAnswererBlankButtonCommand.ExecuteAsync();
        }

        private void MyMpRadioButton_OnTapped(object sender, EventArgs e)
        {
            MyMpRadioButton.IsChecked = true;
        }
        
        private void AnotherMpRadioButton_OnTapped(object sender, EventArgs e)
        {
            AnotherMpRadioButton.IsChecked = true;
        }
        
        private void PublicAuthorityRadioButton_OnTapped(object sender, EventArgs e)
        {
            PublicAuthorityRadioButton.IsChecked = true;
        }
        private void DontKnowRadioButton_OnTapped(object sender, EventArgs e)
        {
            DontKnowRadioButton.IsChecked = true;
        }
        
    }
}
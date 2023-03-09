using System;
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
        }

        private void InApp_OnTapped(object sender, EventArgs e)
        {
            InAppRadioButton.IsChecked = true;
        }
        
        private void InParliament_OnTapped(object sender, EventArgs e)
        {
            InParliamentRadioButton.IsChecked = true;
        }
        
        private void DontKnow_OnTapped(object sender, EventArgs e)
        {
            DontKnowRadioButton.IsChecked = true;
        }
    }
}
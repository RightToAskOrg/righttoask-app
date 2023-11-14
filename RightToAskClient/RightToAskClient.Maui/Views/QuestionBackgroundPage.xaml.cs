using System;
using System.Threading;
using System.Threading.Tasks;
using RightToAskClient.Maui.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionBackgroundPage : ContentPage
    {
        public QuestionBackgroundPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SupportingInfoEditor.Focus();
        }

        protected override void OnDisappearing()
        {
            SupportingInfoEditor.Unfocus();
            base.OnDisappearing();
        }

        private void InputView_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int length = e.NewTextValue.Length;
            EditorTextNumber.Text = length + "/" + SupportingInfoEditor.MaxLength;
        }
    }
}
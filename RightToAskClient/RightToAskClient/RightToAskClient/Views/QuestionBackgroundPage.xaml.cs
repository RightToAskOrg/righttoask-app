using System;
using System.Threading;
using System.Threading.Tasks;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
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
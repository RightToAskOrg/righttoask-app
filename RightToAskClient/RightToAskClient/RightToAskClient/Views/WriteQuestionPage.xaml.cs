using System;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WriteQuestionPage : ContentPage
    {
        public WriteQuestionPage()
        {
            InitializeComponent();
        }

        private void LifecycleEffect_OnLoaded(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, KeywordEntry))
            {
                KeywordEntry.Focus();
            }
        }

        protected override void OnDisappearing()
        {
            KeywordEntry.Unfocus();
            base.OnDisappearing();
        }

        private void KeywordEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // string EditorNewText = e.NewTextValue;
            
        }
    }
}
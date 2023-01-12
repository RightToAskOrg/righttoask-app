using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WriteQuestionPage : ContentPage
    {
        public WriteQuestionPage()
        {
            InitializeComponent();
            ClearButton.IsVisible = false;
            EditorProceedButton.IsEnabled = false;
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
            int length = e.NewTextValue.Length;
            ClearButton.IsVisible = length > 0;
            EditorTextNumber.Text = length + "/" + KeywordEntry.MaxLength;
            EditorProceedButton.IsEnabled = length > 0;
        }

        private void WriteQuestionPage_OnSizeChanged(object sender, EventArgs e)
        {
            Questions_HeightSet();
            
            if (Device.RuntimePlatform == Device.iOS)
            {
                var safeArea = On<iOS>().SafeAreaInsets();
                Padding = new Thickness(0, 0, 0, - safeArea.Bottom);
            }
            
        }

        private void ClearButton_OnClicked(object sender, EventArgs e)
        {
            KeywordEntry.Text = "";
            ClearButton.IsVisible = false;
        }

        private void KeywordEntry_FocusedChange(object sender, FocusEventArgs e)
        {
            Questions_HeightSet();
            if(!e.IsFocused)
                QuestionsArea.HeightRequest += 100;
        }

        private void Questions_HeightSet()
        {
            var newHeight = Content.Height - EditorArea.Height - HeaderArea.Height;
            QuestionsArea.HeightRequest = newHeight;
            if (Device.RuntimePlatform == Device.Android)
            {
                QuestionsArea.HeightRequest += 130;
            }
        }

    }
}
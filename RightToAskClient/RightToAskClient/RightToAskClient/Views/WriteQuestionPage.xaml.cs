using System;
using RightToAskClient.ViewModels;
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
            
            var vm = BindingContext as WriteQuestionViewModel;
            vm.RequestUpdate(e.NewTextValue);
        }

        private Double _contentHeight = -1;

        private void WriteQuestionPage_OnSizeChanged(object sender, EventArgs e)
        {
            _contentHeight = Content.Height;
            Questions_HeightSet(KeywordEntry.IsFocused);
            
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
            Questions_HeightSet(e.IsFocused);
        }

        private void Questions_HeightSet(bool isFocused)
        {
            var contentHeight = _contentHeight;
            var editorHeight = EditorArea.Height;

            var newHeight = Math.Max(0, contentHeight - editorHeight);
            if (isFocused)
            {
                newHeight -= GetKeyboardHeight();
            }
            QuestionsArea.HeightRequest = newHeight;
        }

        private Double GetKeyboardHeight()
        {
            if (Device.RuntimePlatform == Device.iOS) 
                return 368;
            return 0;
        }

        private void ReturnHomeButton_OnClicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PopAsync();
        }

        private void EditorProceedButton_OnClicked(object sender, EventArgs e)
        {
            //todo
        }
    }
}
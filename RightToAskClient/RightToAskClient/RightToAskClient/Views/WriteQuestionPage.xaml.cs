using System;
using RightToAskClient.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

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

            if (BindingContext is WriteQuestionViewModel vm)
            {
                vm.RequestUpdate(e.NewTextValue);
            }
        }

        private Double _contentHeight = -1;

        private void WriteQuestionPage_OnSizeChanged(object sender, EventArgs e)
        {
            _contentHeight = Content.Height;
            Questions_HeightSet(KeywordEntry.IsFocused);
            
            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
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
            var query = KeywordEntry.Text;
            if (query != null)
            {
                var vm = BindingContext as WriteQuestionViewModel;
                vm.RequestUpdate(query, true);
            }
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
            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            if (Device.RuntimePlatform == Device.iOS)
            {
                var safeArea = On<iOS>().SafeAreaInsets();
                if (safeArea.Bottom == 0)
                {
                    return 320;
                }
                else
                {
                    return 360;
                }
            }

            return 0;
        }
    }
}
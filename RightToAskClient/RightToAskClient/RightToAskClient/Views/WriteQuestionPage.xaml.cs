using System;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Application = Xamarin.Forms.PlatformConfiguration.iOSSpecific.Application;

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

        private void KeywordEntry_OnFocused(object sender, FocusEventArgs e)
        {
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var density = mainDisplayInfo.Density;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;
            var xamarinWidth = width / density;
            var xamarinHeight = height / density;
            
            var pageHeight = xamarinHeight;
            var headerHeight = HeaderArea.Height;
            var editorHeight = EditorArea.Height;
            var newHeight = pageHeight - (headerHeight + editorHeight);
            QuestionsArea.HeightRequest = newHeight;
        }
    }
}
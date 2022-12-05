using Xamarin.Forms;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.Extensions;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;

namespace RightToAskClient.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        public ContentPage Page { get; set; } = new ContentPage();

        // constructor
        public BaseViewModel()
        {
            PopupLabelText = "TestText";
            HomeButtonCommand = new AsyncCommand(async () =>
            {
                var popup = new TwoButtonPopup(AppResources.GoHomePopupTitle, AppResources.GoHomePopupText, AppResources.CancelButtonText, AppResources.GoHomeButtonText, false);
                var popupResult = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (popup.HasApproved(popupResult))
                {
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
                }
            });
            InfoPopupCommand = new AsyncCommand(async () =>
            {
                //Page.Navigation.ShowPopup(new InfoPopup());
                var popup = new InfoPopup(PopupHeaderText,PopupLabelText, AppResources.OKText);
                _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
            });
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _popupHeaderText = "";
        public string PopupHeaderText
        {
            get => _popupHeaderText;
            set => SetProperty(ref _popupHeaderText, value);
        }
        
        private string _popupLabelText = "";
        public string PopupLabelText
        {
            get => _popupLabelText;
            set => SetProperty(ref _popupLabelText, value);
        }

        private string _reportLabelText = "";
        public string ReportLabelText
        {
            get => _reportLabelText;
            set => SetProperty(ref _reportLabelText, value);
        }

        // commands
        public IAsyncCommand HomeButtonCommand { get; }
        public IAsyncCommand InfoPopupCommand { get; }
    }
}

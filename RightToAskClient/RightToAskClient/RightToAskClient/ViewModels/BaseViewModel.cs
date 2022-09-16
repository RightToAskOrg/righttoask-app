using Xamarin.Forms;
using Xamarin.CommunityToolkit.ObjectModel;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.Extensions;
using RightToAskClient.Resx;

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
                var popup = new TwoButtonPopup(this, AppResources.GoHomePopupTitle, AppResources.GoHomePopupText, AppResources.CancelButtonText, AppResources.GoHomeButtonText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (ApproveButtonClicked)
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                }
            });
            InfoPopupCommand = new AsyncCommand(async () =>
            {
                //Page.Navigation.ShowPopup(new InfoPopup());
                var popup = new InfoPopup(PopupHeaderText,PopupLabelText, AppResources.OKText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            });
            ApproveCommand = new Command(() =>
            {
                ApproveButtonClicked = true;
                CancelButtonClicked = false;
                //Dismiss();
            });
            CancelCommand = new Command(() =>
            {
                ApproveButtonClicked = false;
                CancelButtonClicked = true;
            });
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string title = string.Empty;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
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

        // booleans for feedback from generic popup button presses
        public bool CancelButtonClicked { get; set; } = false;
        public bool ApproveButtonClicked { get; set; } = false;

        // commands
        public IAsyncCommand HomeButtonCommand { get; }
        public IAsyncCommand InfoPopupCommand { get; }
        public Command ApproveCommand { get; set; }
        public Command CancelCommand { get; set; }
    }
}

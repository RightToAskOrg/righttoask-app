using Xamarin.Forms;
using Xamarin.CommunityToolkit.ObjectModel;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.Extensions;

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
                string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go home? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
                if (result == "Yes, I'm sure.")
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                }
            });
            InfoPopupCommand = new AsyncCommand(async () =>
            {
                //Page.Navigation.ShowPopup(new InfoPopup());
                var popup = new InfoPopup(PopupLabelText);
                _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
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

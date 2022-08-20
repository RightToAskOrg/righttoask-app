using RightToAskClient.Resx;
using RightToAskClient.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private bool _showMyQuestions = false;
        public bool ShowMyQuestions
        {
            get => _showMyQuestions;
            set => SetProperty(ref _showMyQuestions, value);
        }

        private bool _showTrendingMyElectorate = false;
        public bool ShowTrendingMyElectorate
        {
            get => _showTrendingMyElectorate;
            set => SetProperty(ref _showTrendingMyElectorate, value);
        }

        /*
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _ = SetProperty(ref _searchText, value);
                App.ReadingContext.Filters.SearchKeyword = value;
                if (!string.IsNullOrEmpty(_searchText))
                {
                    App.ReadingContext.IsReadingOnly = true;
                }
            }
        } */

        public MainPageViewModel()
        {
            PopupLabelText = AppResources.MainPagePopupText;
            
            MessagingCenter.Subscribe<FindMPsViewModel>(this, Constants.ElectoratesKnown, (sender) =>
            {
                ShowTrendingMyElectorate = true;
                MessagingCenter.Unsubscribe<FindMPsViewModel>(this, Constants.ElectoratesKnown);
            });
            // commands
            TrendingNowButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.TrendingNow = true;
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            ExpiringSoonButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.TrendingNow = true;
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            DraftingButtonCommand = new AsyncCommand(async () =>
            {
                if (App.ReadingContext.ShowHowToPublishPopup)
                {
                    var popup = new HowToPublishPopup(); // this instance uses a model instead of a VM
                    _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    App.ReadingContext.ShowHowToPublishPopup = false;
                    Preferences.Set(Constants.ShowHowToPublishPopup, false);
                }
                App.ReadingContext.IsReadingOnly = false;
                await Shell.Current.GoToAsync($"{nameof(SecondPage)}");
            });
            AdvancedSearchButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(AdvancedSearchFiltersPage)}").ContinueWith((_) =>
                {
                    MessagingCenter.Send<MainPageViewModel>(this, "MainPage");
                });
            });
            SearchButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
        }

        // commands
        public IAsyncCommand TrendingNowButtonCommand { get; }
        public IAsyncCommand ExpiringSoonButtonCommand { get; }
        public IAsyncCommand DraftingButtonCommand { get; }
        public IAsyncCommand AdvancedSearchButtonCommand { get; }
        public IAsyncCommand SearchButtonCommand { get; }
    }
}

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
        }

        public MainPageViewModel()
        {
            // check bools
            ShowMyQuestions = false;
            ShowTrendingMyElectorate = false;
            if (App.ReadingContext.ThisParticipant.MPsKnown)
            {
                ShowTrendingMyElectorate = true;
            }
            // find out if they have upvoted or created questions before
            if (App.ReadingContext.ThisParticipant.HasQuestions)
            {
                ShowMyQuestions = true;
            }

            PopupLabelText = AppResources.MainPagePopupText;
            // commands
            Top10ButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.TopTen = true;
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            ExpiringSoonButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.TopTen = true;
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
                    Preferences.Set("ShowHowToPublishPopup", false);
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
                //await Shell.Current.GoToAsync($"{nameof(ReadingPage)}").ContinueWith(async (_) =>
                //{
                //    await Shell.Current.GoToAsync($"{nameof(AdvancedSearchFiltersPage)}").ContinueWith((_) =>
                //    {
                //        MessagingCenter.Send<MainPageViewModel>(this, "MainPage");
                //    });
                //});
                // TODO: Start the page with filters expanded and have the keyword entered in filters
            });
            SearchButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
        }

        // commands
        public IAsyncCommand Top10ButtonCommand { get; }
        public IAsyncCommand ExpiringSoonButtonCommand { get; }
        public IAsyncCommand DraftingButtonCommand { get; }
        public IAsyncCommand AdvancedSearchButtonCommand { get; }
        public IAsyncCommand SearchButtonCommand { get; }
    }
}

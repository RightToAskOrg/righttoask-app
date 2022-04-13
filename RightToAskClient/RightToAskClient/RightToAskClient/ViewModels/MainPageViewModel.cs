using RightToAskClient.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
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
                App.ReadingContext.IsReadingOnly = false;
                await Shell.Current.GoToAsync($"{nameof(SecondPage)}");
            });
            AdvancedSearchButtonCommand = new AsyncCommand(async () =>
            {
                App.ReadingContext.IsReadingOnly = true;
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}").ContinueWith(async(_) =>{
                    await Shell.Current.GoToAsync($"{nameof(AdvancedSearchFiltersPage)}");
                });
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

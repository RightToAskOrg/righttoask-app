using RightToAskClient.Resx;
using RightToAskClient.Views;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private bool _showMyQuestions;
        public bool ShowMyQuestions
        {
            get => _showMyQuestions;
            set
            {
                _showMyQuestions = value;
                OnPropertyChanged();
            }
        }

        private bool _showTrendingMyElectorate;
        public bool ShowTrendingMyElectorate
        {
            get => _showTrendingMyElectorate;
            set
            {
                _showTrendingMyElectorate = value;
                OnPropertyChanged();
            }
        }

        private bool _showQuestionsForMe;
        public bool ShowQuestionsForMe
        {
            get => _showQuestionsForMe;
            set
            {
                _showQuestionsForMe = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            PopupLabelText = AppResources.MainPagePopupText;
            
            MessagingCenter.Subscribe<FindMPsViewModel>(this, Constants.ElectoratesKnown, (sender) =>
            {
                ShowTrendingMyElectorate = true;
                MessagingCenter.Unsubscribe<FindMPsViewModel>(this, Constants.ElectoratesKnown);
            });
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.HasQuestions, (sender) =>
            {
                ShowMyQuestions = true;
                MessagingCenter.Unsubscribe<FindMPsViewModel>(this, Constants.HasQuestions);
            });
            MessagingCenter.Subscribe<MPRegistrationVerificationViewModel>(this, Constants.IsVerifiedMPAccount, (sender) =>
            {
                ShowQuestionsForMe = true;
                MessagingCenter.Unsubscribe<MPRegistrationVerificationViewModel>(this, Constants.IsVerifiedMPAccount);
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
                    _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
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

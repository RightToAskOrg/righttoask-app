using RightToAskClient.Models;
using RightToAskClient.Views;
using System.Collections.Generic;
using System.Linq;
using Xamarin.CommunityToolkit.ObjectModel;
using RightToAskClient.Resx;
using Xamarin.Forms;
using System.Threading.Tasks;
using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        /* FIXME It would make a lot more sense to set this up with either an empty constructor
         * (for the ReadingContext filters) or an explicit/new one (for a fresh question etc).
         * The instance is only used in the Metadata page. Not clear that this is logically correct.
         * Why not just use the data from the GlobalFilters, since we only ever access the Metadata
         * page with those filters?
         */
        private static FilterViewModel? _instance;
        public static FilterViewModel Instance => _instance ??= new FilterViewModel();

        // properties
        // private static FilterChoices GlobalFilterChoices => App.ReadingContext.Filters;

        public ClickableListViewModel AnsweringMPsOther { get; }
        public ClickableListViewModel AnsweringMPsMine { get; }
        public ClickableListViewModel AnsweringAuthorities { get; }
        public ClickableListViewModel AskingMPsOther { get; }
        public ClickableListViewModel AskingMPsMine { get; }
        public ClickableListViewModel Committees { get; }
        

        private string _otherRightToAskUserText = "";
        public string OtherRightToAskUserText
        {
            get => _otherRightToAskUserText;
            set => SetProperty(ref _otherRightToAskUserText, value);
        }

        private SelectableList<Person> _selectableParticipants; 

        public string QuestionWriter
        {
            get
            {
                var writers = _selectableParticipants.SelectedEntities;
                return writers.Any() ? writers.FirstOrDefault().ToString() : "";
            }
        }

        private string _questionWriterSearchText = "";

        public string QuestionWriterSearchText
        {
            get => _questionWriterSearchText;
            set => SetProperty(ref _questionWriterSearchText, value);
        }
        
        // for the metadata page
        public bool OthersCanAddAnswerers
        {
            get => QuestionViewModel.Instance.OthersCanAddQuestionAnswerers; 
            set => QuestionViewModel.Instance.OthersCanAddQuestionAnswerers = value;
        }

        public bool OthersCanAddAskers
        {
            get => QuestionViewModel.Instance.OthersCanAddQuestionAskers; 
            set => QuestionViewModel.Instance.OthersCanAddQuestionAskers = value;
        }

        private bool _answerInApp;
        public bool AnswerInApp
        {
            get => QuestionViewModel.Instance.AnswerInApp;
            set
            {
                SetProperty(ref _answerInApp, value);
                QuestionViewModel.Instance.AnswerInApp = _answerInApp;
            }
        }

        private string _keyword = "";
        public string Keyword
        {
            get => _keyword;
            set
            {
                var changed = SetProperty(ref _keyword, value);
                if (changed)
                {
                    App.GlobalFilterChoices.SearchKeyword = _keyword;
                }
            }
        }

        public FilterViewModel()
        {
            PopupLabelText = AppResources.FiltersPopupText;
            MessagingCenter.Subscribe<QuestionViewModel>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, "UpdateFilters");
            });
            MessagingCenter.Subscribe<SelectableListViewModel>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
                // Normally we'd want to unsubscribe to prevent multiple instances of the subscriber from happening,
                // but because these listeners happen when popping back to this page from a selectableList page we want to keep the listener/subscriber
                // active to update all of the lists/filters on this page with the newly selected data
                //MessagingCenter.Unsubscribe<SelectableListViewModel>(this, "UpdateFilters");
            });
            /*
            MessagingCenter.Subscribe<MainPageViewModel>(this, "MainPage", (sender) =>
            {
                CameFromMainPage = true;
                MessagingCenter.Unsubscribe<MainPageViewModel>(this, "MainPage");
            });
            */

            Title = AppResources.AdvancedSearchButtonText; 
            ReinitData(); // to set the display strings

            AnsweringMPsMine = new ClickableListViewModel(App.GlobalFilterChoices.AnsweringMPsListsMine)
            {
                // FIXME This isn't quite right when the MPs are not known - seems to push a 
                // list to choose from and then go to a reading page, rather than popping on completion
                // and returning to the search page.
                // See Issue #105
                EditListCommand = new Command(() =>
                {
                    _ = EditSelectedAnsweringMPsMineClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage); // Sends this view model
                    });
                }),
                Heading = AppResources.MyMPButtonText
            };
            AnsweringMPsOther = new ClickableListViewModel(App.GlobalFilterChoices.AnsweringMPsListsNotMine)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditOtherSelectedAnsweringMPsClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                    });
                }),
                Heading = AppResources.OtherMP,
            };
            AnsweringAuthorities = new ClickableListViewModel(App.GlobalFilterChoices.AuthorityLists)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditAuthoritiesClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                    });
                }),
                Heading = AppResources.AuthorityLabel,
            };
            AskingMPsMine = new ClickableListViewModel(App.GlobalFilterChoices.AskingMPsListsMine)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditSelectedAskingMPsClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                    });
                }),
                Heading = AppResources.MyMPButtonText
            };
            AskingMPsOther = new ClickableListViewModel(App.GlobalFilterChoices.AskingMPsListsNotMine)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditOtherSelectedAskingMPsClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                    });
                }),
                Heading = AppResources.OtherMP
            };
            Committees = new ClickableListViewModel(App.GlobalFilterChoices.CommitteeLists)
            {
                EditListCommand = new Command(() =>
                {
                    _ = NavigationUtils.EditCommitteesClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                    });
                }),
                Heading = AppResources.ParliamentaryCommitteeText
            };
            // commands
            AnsweringMPsMineFilterCommand = new Command(() =>
            {
                _ = EditSelectedAnsweringMPsMineClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage); // Sends this view model
                });
            });
            AskingMPsMineFilterCommand = new Command(() =>
            {
                _ = EditSelectedAskingMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                  });
            });
            AnsweringAuthoritiesFilterCommand = new Command(() =>
            {
                _ = EditAuthoritiesClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                  });
            });
            OtherAnsweringMPsFilterCommand = new Command(() =>
            {
                _ = EditOtherSelectedAnsweringMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                  });
            });
            OtherAskingMPsFilterCommand = new Command(() =>
            {
                _ = EditOtherSelectedAskingMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                  });
            });
            WrittenByRightToAskUserCommand = new AsyncCommand(async () =>
            {
                _ = SearchUserWrittenByClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, Constants.GoBackToAdvancedSearchPage);
                  });
            });
            NotSureCommand = new Command(() =>
            {
                // not implemented yet
            });
            SearchCommand = new Command(() =>
            {
                ApplyFiltersAndSearch();
            });
            BackCommand = new AsyncCommand(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });
            ForceUpdateSizeCommand = new Command(() =>
            {
                ReinitData();
            });
            ToDetailsPageCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
            });
        }



        // commands
        public Command AnsweringMPsMineFilterCommand { get; }
        public Command AskingMPsMineFilterCommand { get; }
        public Command AnsweringAuthoritiesFilterCommand { get; }
        public Command OtherAnsweringMPsFilterCommand { get; }
        public Command OtherAskingMPsFilterCommand { get; }
        public AsyncCommand WrittenByRightToAskUserCommand { get; }
        public Command NotSureCommand { get; }
        public Command SearchCommand { get; }
        public IAsyncCommand BackCommand { get; }
        public Command ForceUpdateSizeCommand { get; }
        public IAsyncCommand ToDetailsPageCommand { get; }

        // helper methods
        public void ReinitData()
        {
            // set the keyword
            Keyword = App.GlobalFilterChoices.SearchKeyword;

            // TODO Ideally, we shouldn't have to do this manually,
            // but I don't see a more elegant way at the moment.
            // I tried raising it in SelectableList.cs but that didn't work.
            OnPropertyChanged("PublicAuthoritiesText");
            OnPropertyChanged("AnsweringMPsOther");
            OnPropertyChanged("AnsweringMPsMine");
            OnPropertyChanged("AnsweringAuthorities");  
            OnPropertyChanged("AskingMPsOther");
            OnPropertyChanged("AskingMPsMine");
            OnPropertyChanged("SelectedCommittees");
            OnPropertyChanged("QuestionWriter");
            
        }

        public string CreateTextGivenListEntities(IEnumerable<Entity> entityList)
        {
            return string.Join(", ", entityList.Select(e => e.ShortestName));
        }

        private async Task EditSelectedAnsweringMPsMineClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAnsweringMPsExploringPage();
            }
        }

        private async Task EditAuthoritiesClicked()
        {
            var message = "Choose others to add";

            var departmentExploringPage
                = new SelectableListPage(App.GlobalFilterChoices.AuthorityLists, message);
            await Application.Current.MainPage.Navigation.PushAsync(departmentExploringPage);
        }


        private async Task EditOtherSelectedAnsweringMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage();
            }
        }

        private async Task EditOtherSelectedAskingMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsNotMineSelectableListPageAsync();
            }
        }

        private async Task EditSelectedAskingMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAskingMPsExploringPage();
            }
        }

        private async Task SearchUserWrittenByClicked()
        {
            var searchString = QuestionWriterSearchText;

            var searchResults = await RTAClient.SearchUser(searchString);
            if (searchResults.Failure)
            {
                ReportLabelText = "Error searching for user " + searchString + ". ";
                if (searchResults is ErrorResult<List<ServerUser>> errorResult)
                {
                    ReportLabelText += errorResult;
                }
            }
            else if (!searchResults.Data.Any())
            {
                ReportLabelText = AppResources.NoParticipantsFoundText;
            }
            else
            {
                var matchingParticipants = searchResults.Data.Select(u => new Person(u)).ToList();
                _selectableParticipants = new SelectableList<Person>(matchingParticipants);
                App.GlobalFilterChoices.QuestionWriterLists = _selectableParticipants;
                var participantsSearchSelectionPage
                    = new SelectableListPage(_selectableParticipants, AppResources.ChooseParticipantsText, true);
                await Application.Current.MainPage.Navigation.PushAsync(participantsSearchSelectionPage).ContinueWith( (_) =>
                    MessagingCenter.Send(this, "GoToReadingPageWithSingleQuestionWriter")
                );
            }
        }
        private async void ApplyFiltersAndSearch()
        {
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}

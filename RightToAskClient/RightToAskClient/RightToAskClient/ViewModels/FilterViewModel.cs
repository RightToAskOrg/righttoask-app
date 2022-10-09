using RightToAskClient.Models;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.CommunityToolkit.ObjectModel;
using RightToAskClient.Resx;
using Xamarin.Forms;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;

namespace RightToAskClient.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        /* FIXME It would make a lot more sense to set this up with either an empty constructor
         * (for the ReadingContext filters) or an explicit/new one (for a fresh question etc).
         * The instance is only used in the Metadata page. Not clear that this is logically correct.
         */
        private static FilterViewModel? _instance;
        public static FilterViewModel Instance => _instance ??= new FilterViewModel();

        // properties
        private FilterChoices globalFilterChoices => App.ReadingContext.Filters;

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
                if (writers != null && writers.Any())
                {
                    return writers.FirstOrDefault().ToString();
                }

                return "";
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
            set =>
                // SetProperty(ref _othersCanAddAnswerers, value);
                // QuestionViewModel.Instance.WhoShouldAnswerItPermissions = _othersCanAddAnswerers ? RTAPermissions.Others : RTAPermissions.WriterOnly;
                QuestionViewModel.Instance.OthersCanAddQuestionAnswerers = value;
            // OnPropertyChanged();
        }

        public bool OthersCanAddAskers
        {
            get => QuestionViewModel.Instance.OthersCanAddQuestionAskers; 
            set =>
                // SetProperty(ref _othersCanAddAskers, value);
                // QuestionViewModel.Instance.WhoShouldAskItPermissions = _othersCanAddAskers ? RTAPermissions.Others : RTAPermissions.WriterOnly;
                QuestionViewModel.Instance.OthersCanAddQuestionAskers = value;
            // OnPropertyChanged();
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

        // public string SelectedAnsweringMPsText => CreateTextGivenListEntities(FilterChoices.SelectedAnsweringMPsNotMine.ToList());

        public bool CameFromMainPage;

        private string _keyword = "";
        public string Keyword
        {
            get => _keyword;
            set
            {
                var changed = SetProperty(ref _keyword, value);
                if (changed)
                {
                    App.ReadingContext.Filters.SearchKeyword = _keyword;
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
            MessagingCenter.Subscribe<MainPageViewModel>(this, "MainPage", (sender) =>
            {
                CameFromMainPage = true;
                MessagingCenter.Unsubscribe<MainPageViewModel>(this, "MainPage");
            });

            Title = AppResources.AdvancedSearchButtonText; 
            ReinitData(); // to set the display strings

            AnsweringMPsMine = new ClickableListViewModel(globalFilterChoices.AnsweringMPsListsMine)
            {
                // FIXME This isn't quite right when the MPs are not known - seems to push a 
                // list to choose from and then go to a reading page, rather than popping on completion
                // and returning to the search page.
                // See Issue #105
                EditListCommand = new Command(() =>
                {
                    _ = EditSelectedAnsweringMPsMineClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "FromFiltersPage"); // Sends this view model
                    });
                }),
                Heading = AppResources.MyMPButtonText
            };
            AnsweringMPsOther = new ClickableListViewModel(globalFilterChoices.AnsweringMPsListsNotMine)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditOtherSelectedAnsweringMPsClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "FromFiltersPage");
                    });
                }),
                Heading = AppResources.OtherMP,
            };
            AnsweringAuthorities = new ClickableListViewModel(globalFilterChoices.AuthorityLists)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditAuthoritiesClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "FromFiltersPage");
                    });
                }),
                Heading = AppResources.AuthorityLabel,
            };
            AskingMPsMine = new ClickableListViewModel(globalFilterChoices.AskingMPsListsMine)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditSelectedAskingMPsClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "FromFiltersPage");
                    });
                }),
                Heading = AppResources.MyMPButtonText
            };
            AskingMPsOther = new ClickableListViewModel(globalFilterChoices.AskingMPsListsNotMine)
            {
                EditListCommand = new Command(() =>
                {
                    _ = EditOtherSelectedAskingMPsClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "FromFiltersPage");
                    });
                }),
                Heading = AppResources.OtherMP
            };
            Committees = new ClickableListViewModel(globalFilterChoices.CommitteeLists)
            {
                EditListCommand = new Command(() =>
                {
                    _ = NavigationUtils.EditCommitteesClicked().ContinueWith((_) =>
                    {
                        MessagingCenter.Send(this, "FromFiltersPage");
                    });
                }),
                Heading = AppResources.ParliamentaryCommitteeText
            };
            // commands
            AnsweringMPsMineFilterCommand = new Command(() =>
            {
                _ = EditSelectedAnsweringMPsMineClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage"); // Sends this view model
                });
            });
            AskingMPsMineFilterCommand = new Command(() =>
            {
                _ = EditSelectedAskingMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage");
                  });
            });
            AnsweringAuthoritiesFilterCommand = new Command(() =>
            {
                _ = EditAuthoritiesClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage");
                  });
            });
            OtherAnsweringMPsFilterCommand = new Command(() =>
            {
                _ = EditOtherSelectedAnsweringMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage");
                  });
            });
            OtherAskingMPsFilterCommand = new Command(() =>
            {
                _ = EditOtherSelectedAskingMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage");
                  });
            });
            WrittenByRightToAskUserCommand = new AsyncCommand(async () =>
            {
                _ = SearchUserWrittenByClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage");
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
                if (CameFromMainPage)
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                }
                else
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }
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
            Keyword = App.ReadingContext.Filters.SearchKeyword;

            // get lists of data
            //SelectedAskingMPsList = FilterChoices.SelectedAskingMPsNotMine.ToList();
            //SelectedAnsweringMPsList = FilterChoices.SelectedAnsweringMPsNotMine.ToList();
            //SelectedAskingMyMPsList = FilterChoices.SelectedAskingMPsMine.ToList();
            //SelectedAnsweringMyMPsList = FilterChoices.SelectedAnsweringMPsMine.ToList();
            // PublicAuthoritiesList = FilterChoices.SelectedAuthorities.ToList();
            // OtherRightToAskUserList = FilterChoices.SelectedAskingUsers.ToList();
            // CommitteeList = FilterChoices.SelectedCommittee.ToList();

            // create strings from those lists
            //SelectedAskingMPsText = CreateTextGivenListEntities(SelectedAskingMPsList);
            //SelectedAnsweringMPsText = CreateTextGivenListEntities(SelectedAnsweringMPsList);
            //SelectedAskingMyMPsText = CreateTextGivenListEntities(SelectedAskingMyMPsList);
            // SelectedAnsweringMyMPsText = CreateTextGivenListEntities(SelectedAnsweringMyMPsList);
            // PublicAuthoritiesText = CreateTextGivenListEntities(PublicAuthoritiesList);
            // This line is necessary for updating the views.
            // TODO Ideally, we shouldn't have to do this manually,
            // but I don't see a more elegant way at the moment.
            // I tried raising it in SelectableList.cs but that didn't work.
            OnPropertyChanged("SelectedAskingMPsText");
            OnPropertyChanged("SelectedAnsweringMPsText");
            OnPropertyChanged("SelectedAskingMyMPsText");
            OnPropertyChanged("SelectedAnsweringMyMPsText");
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
                = new SelectableListPage(App.ReadingContext.Filters.AuthorityLists, message);
            await App.Current.MainPage.Navigation.PushAsync(departmentExploringPage);
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
            if (!string.IsNullOrEmpty(searchResults.Err))
            {
                ReportLabelText = searchResults.Err ?? string.Empty;
            }
            else if (!searchResults.Ok.Any())
            {
                ReportLabelText = AppResources.NoParticipantsFoundText;
            }
            else
            {
                var matchingParticipants = searchResults.Ok.Select(u => new Person(u)).ToList();
                _selectableParticipants = new SelectableList<Person>(matchingParticipants);
                App.ReadingContext.Filters.QuestionWriterLists = _selectableParticipants;
                var participantsSearchSelectionPage
                    = new SelectableListPage(_selectableParticipants, AppResources.ChooseParticipantsText, true);
                await App.Current.MainPage.Navigation.PushAsync(participantsSearchSelectionPage).ContinueWith( (_) =>
                    MessagingCenter.Send(this, "GoToReadingPageWithSingleQuestionWriter")
                );
            }
        }
        private async void ApplyFiltersAndSearch()
        {
            if (CameFromMainPage)
            {
                await Shell.Current.GoToAsync(nameof(ReadingPage));
            }
            else
            {
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}

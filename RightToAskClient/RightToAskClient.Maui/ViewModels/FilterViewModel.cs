using RightToAskClient.Maui.Models;
using RightToAskClient.Maui.Views;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using RightToAskClient.Maui.Resx;
using System.Threading.Tasks;
using RightToAskClient.Maui.Helpers;
using RightToAskClient.Maui.HttpClients;
using RightToAskClient.Maui.Models.ServerCommsData;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.Input;

namespace RightToAskClient.Maui.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        public FilterChoices FilterChoices; 
        
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
        
        public string Keyword
        {
            get => FilterChoices.SearchKeyword;
            set
            {
                FilterChoices.SearchKeyword = value;
            }
        }

        public FilterViewModel(FilterChoices filterChoice) 
        {
            FilterChoices = filterChoice;
            
            PopupLabelText = AppResources.FiltersPopupText;
            MessagingCenter.Subscribe<QuestionViewModel>(this, Constants.UpdateFilters, (sender) =>
            {
                ReinitData();
                MessagingCenter.Unsubscribe<QuestionViewModel>(this, Constants.UpdateFilters);
            });
            MessagingCenter.Subscribe<SelectableListViewModel>(this, Constants.UpdateFilters, (sender) =>
            {
                ReinitData();
                // Normally we'd want to unsubscribe to prevent multiple instances of the subscriber from happening,
                // but because these listeners happen when popping back to this page from a selectableList page we want to keep the listener/subscriber
                // active to update all of the lists/filters on this page with the newly selected data
                //MessagingCenter.Unsubscribe<SelectableListViewModel>(this, Constants.UpdateFilters);
            });

            Title = AppResources.AdvancedSearchButtonText; 
            ReinitData(); // to set the display strings

            AnsweringMPsMine = new ClickableListViewModel(FilterChoices.AnsweringMPsListsMine)
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
            AnsweringMPsOther = new ClickableListViewModel(FilterChoices.AnsweringMPsListsNotMine)
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
            AnsweringAuthorities = new ClickableListViewModel(FilterChoices.AuthorityLists)
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
            AskingMPsMine = new ClickableListViewModel(FilterChoices.AskingMPsListsMine)
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
            AskingMPsOther = new ClickableListViewModel(FilterChoices.AskingMPsListsNotMine)
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
            Committees = new ClickableListViewModel(FilterChoices.CommitteeLists)
            {
                EditListCommand = new Command(() =>
                {
                    _ = NavigationUtils.EditCommitteesClicked(FilterChoices.CommitteeLists).ContinueWith((_) =>
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
            WrittenByRightToAskUserCommand = new AsyncRelayCommand (async () =>
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
                MessagingCenter.Send(this, Constants.RefreshQuestionList); // Sends this view model
            });
            ClearSearchCommand = new Command(() =>
            {
                FilterChoices.RemoveAllSelections();
                ReinitData();
                
                // ApplyFiltersAndSearch();
                MessagingCenter.Send(this, Constants.UpdateFilters); // Sends this view model
            });
            BackCommand = new AsyncRelayCommand (async () =>
            {
                SearchCommand.Execute(true);
            });
            ForceUpdateSizeCommand = new Command(() =>
            {
                //TODO:
               // ReinitData();
            });
            ToDetailsPageCommand = new AsyncRelayCommand (async () =>
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
        public AsyncRelayCommand  WrittenByRightToAskUserCommand { get; }
        public Command NotSureCommand { get; }
        public Command SearchCommand { get; }
        public Command ClearSearchCommand { get; }
        public IAsyncRelayCommand  BackCommand { get; }
        public Command ForceUpdateSizeCommand { get; }
        public IAsyncRelayCommand  ToDetailsPageCommand { get; }

        // helper methods
        public void ReinitData()
        {
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
            OnPropertyChanged("Keyword");
            OnPropertyChanged("AskingMPsOther.ListDisplayText");
        }

        private async Task EditSelectedAnsweringMPsMineClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAnsweringMPsExploringPage(
                    IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown,
                    FilterChoices.AnsweringMPsListsMine);
            }
        }

        private async Task EditAuthoritiesClicked()
        {
            var message = "Choose others to add";

            var departmentExploringPage
                = new SelectableListPage(FilterChoices.AuthorityLists, message);
            await Application.Current.MainPage.Navigation.PushAsync(departmentExploringPage);
        }


        private async Task EditOtherSelectedAnsweringMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAnsweringMPsNotMineSelectableListPage(FilterChoices.AnsweringMPsListsNotMine);
            }
        }

        private async Task EditOtherSelectedAskingMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsNotMineSelectableListPageAsync(FilterChoices.AskingMPsListsNotMine);
            }
        }

        private async Task EditSelectedAskingMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAskingMPsExploringPage(
                    IndividualParticipant.getInstance().ProfileData.RegistrationInfo.ElectoratesKnown,
                    FilterChoices.AskingMPsListsMine);
            }
        }

        // **Note to devs: This function is not currently used, but contains all the logic
        // necessary for searching for an existing username when registering.
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
                FilterChoices.QuestionWriterLists = _selectableParticipants;
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

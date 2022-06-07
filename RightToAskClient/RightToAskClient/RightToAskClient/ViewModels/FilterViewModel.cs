using RightToAskClient.Controls;
using RightToAskClient.Models;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Xamarin.CommunityToolkit.ObjectModel;
using RightToAskClient.Resx;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace RightToAskClient.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        private static FilterViewModel? _instance;
        public static FilterViewModel Instance => _instance ??= new FilterViewModel();

        // properties
        // public FilterDisplayTableView FilterDisplay = new FilterDisplayTableView();
        public FilterChoices FilterChoices => App.ReadingContext.Filters;

        public List<string> CommitteeList = new List<string>();
        private string _committeeText = "";
        public string CommitteeText
        {
            get => _committeeText;
            set => SetProperty(ref _committeeText, value);
        }

        public List<Person> OtherRightToAskUserList = new List<Person>();
        private string _otherRightToAskUserText = "";
        public string OtherRightToAskUserText
        {
            get => _otherRightToAskUserText;
            set => SetProperty(ref _otherRightToAskUserText, value);
        }

        // public List<Authority> PublicAuthoritiesList = new List<Authority>();
        // private string _publicAuthoritiesText = "";
        // VT Note to Matt: See how I've refactored this so that there's no need to update -
        // it isn't really a separate data structure at all, just a formatted way of reading
        // the SelectedAuthorities.
        public string PublicAuthoritiesText => CreateTextGivenListEntities(FilterChoices.SelectedAuthorities.ToList());
        public string SelectedAnsweringMyMPsText => CreateTextGivenListEntities(FilterChoices.SelectedAnsweringMPsMine.ToList());
        public string SelectedAskingMyMPsText => CreateTextGivenListEntities(FilterChoices.SelectedAskingMPsMine.ToList());
        public string SelectedAskingMPsText => CreateTextGivenListEntities(FilterChoices.SelectedAskingMPsNotMine.ToList());
        public string SelectedAnsweringMPsText => CreateTextGivenListEntities(FilterChoices.SelectedAnsweringMPsNotMine.ToList());

        public bool CameFromMainPage = false;

        private string _keyword = "";
        public string Keyword
        {
            get => _keyword;
            set
            {
                bool changed = SetProperty(ref _keyword, value);
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
            MessagingCenter.Subscribe<ExploringPage>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
                //MessagingCenter.Unsubscribe<ExploringPage>(this, "UpdateFilters");
            });
            MessagingCenter.Subscribe<ExploringPageWithSearch>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
                //MessagingCenter.Unsubscribe<ExploringPageWithSearch>(this, "UpdateFilters");
            });

            MessagingCenter.Subscribe<MainPageViewModel>(this, "MainPage", (sender) =>
            {
                CameFromMainPage = true;
                MessagingCenter.Unsubscribe<MainPageViewModel>(this, "MainPage");
            });

            Title = AppResources.AdvancedSearchButtonText; 
            ReinitData(); // to set the display strings

            // commands
            AnsweringMPsFilterCommand = new Command(() =>
            {
                _ = EditSelectedAnsweringMPsClicked().ContinueWith((_) =>
                  {
                      MessagingCenter.Send(this, "FromFiltersPage"); // Sends this view model
                });
            });
            AskingMPsFilterCommand = new Command(() =>
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
            RightToAskUserCommand = new Command(() =>
            {
                // not implemented yet
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
        public Command AnsweringMPsFilterCommand { get; }
        public Command AskingMPsFilterCommand { get; }
        public Command AnsweringAuthoritiesFilterCommand { get; }
        public Command OtherAnsweringMPsFilterCommand { get; }
        public Command OtherAskingMPsFilterCommand { get; }
        public Command RightToAskUserCommand { get; }
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
            OtherRightToAskUserList = FilterChoices.SelectedAskingUsers.ToList();
            CommitteeList = FilterChoices.SelectedAskingCommittee.ToList();

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
            OtherRightToAskUserText = CreateTextGivenListEntities(OtherRightToAskUserList);
            CommitteeText = CreateTextGivenListCommittees(CommitteeList);
        }

        public string CreateTextGivenListEntities(IEnumerable<Entity> entityList)
        {
            return String.Join(", ", entityList.Select(e => e.ShortestName));
        }

        // TODO merge into CreateTextGivenListEntities.
        public string CreateTextGivenListCommittees(List<string> committeeList)
        {
            string text = "";
            for (int i = 0; i < committeeList.Count; i++)
            {
                text += committeeList[i].ToString();
                if (i == committeeList.Count - 1)
                {
                    // no comma + space
                    continue;
                }
                else
                {
                    text += ", ";
                }
            }
            return text;
        }

        private async Task EditSelectedAnsweringMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAnsweringMPsExploringPage();
            }
        }

        private async Task EditAuthoritiesClicked()
        {
            string message = "Choose others to add";

            var departmentExploringPage
                // = new ExploringPageWithSearchAndPreSelections(App.ReadingContext.Filters.SelectedAuthorities, message);
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

        private async void ApplyFiltersAndSearch()
        {
            // TODO apply filters to the list of questions
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

using RightToAskClient.Controls;
using RightToAskClient.Models;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        private static FilterViewModel? _instance;
        public static FilterViewModel Instance => _instance ??= new FilterViewModel();

        // properties
        public FilterDisplayTableView FilterDisplay = new FilterDisplayTableView();
        public FilterChoices FilterChoices => App.ReadingContext.Filters;

        public List<string> CommitteeList = new List<string>();
        private string _committeeText = "";
        public string CommitteeText
        {
            get => _committeeText;
            set => SetProperty(ref _committeeText, value);
        }

        public List<Entity> OtherRightToAskUserList = new List<Entity>();
        private string _otherRightToAskUserText = "";
        public string OtherRightToAskUserText
        {
            get => _otherRightToAskUserText;
            set => SetProperty(ref _otherRightToAskUserText, value);
        }

        public List<Authority> PublicAuthoritiesList = new List<Authority>();
        private string _publicAuthoritiesText = "";
        public string PublicAuthoritiesText
        {
            get => _publicAuthoritiesText;
            set => SetProperty(ref _publicAuthoritiesText, value);
        }

        public List<MP> SelectedAnsweringMyMPsList = new List<MP>();
        private string _selectedAnsweringMyMPs = "";
        public string SelectedAnsweringMyMPsText
        {
            get => _selectedAnsweringMyMPs;
            set => SetProperty(ref _selectedAnsweringMyMPs, value);
        }

        public List<MP> SelectedAskingMyMPsList = new List<MP>();
        private string _selectedAskingMyMPs = "";
        public string SelectedAskingMyMPsText
        {
            get => _selectedAskingMyMPs;
            set => SetProperty(ref _selectedAskingMyMPs, value);
        }

        public List<MP> SelectedAskingMPsList = new List<MP>();
        private string _selectedAskingMPs = "";
        public string SelectedAskingMPsText
        {
            get => _selectedAskingMPs;
            set => SetProperty(ref _selectedAskingMPs, value);
        }

        public List<MP> SelectedAnsweringMPsList = new List<MP>();
        private string _selectedAnsweringMPs = "";
        public string SelectedAnsweringMPsText
        {
            get => _selectedAnsweringMPs;
            set => SetProperty(ref _selectedAnsweringMPs, value);
        }

        public FilterViewModel()
        {
            MessagingCenter.Subscribe<ExploringPage>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
            });
            MessagingCenter.Subscribe<ExploringPageWithSearch>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
            });
            MessagingCenter.Subscribe<ExploringPageWithSearchAndPreSelections>(this, "UpdateFilters", (sender) =>
            {
                ReinitData();
            });
            Title = "Advanced Search Page Filters";
            ReinitData(); // to set the display strings

            // commands
            AnsweringMPsFilterCommand = new Command(() =>
            {
                EditSelectedAnsweringMPsClicked();
            });
            AskingMPsFilterCommand = new Command(() =>
            {
                EditSelectedAskingMPsClicked();
            });
            AnsweringAuthoritiesFilterCommand = new Command(() =>
            {
                EditAuthoritiesClicked();
            });
            OtherAnsweringMPsFilterCommand = new Command(() =>
            {
                EditOtherSelectedAnsweringMPsClicked();
            });
            OtherAskingMPsFilterCommand = new Command(() =>
            {
                EditOtherSelectedAskingMPsClicked();
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

        // helper methods
        public void ReinitData()
        {
            // get lists of data
            SelectedAskingMPsList = FilterChoices.SelectedAskingMPs.ToList();
            SelectedAnsweringMPsList = FilterChoices.SelectedAnsweringMPs.ToList();
            SelectedAskingMyMPsList = FilterChoices.SelectedAskingMPsMine.ToList();
            SelectedAnsweringMyMPsList = FilterChoices.SelectedAnsweringMPsMine.ToList();
            PublicAuthoritiesList = FilterChoices.SelectedAuthorities.ToList();
            OtherRightToAskUserList = FilterChoices.SelectedAskingUsers.ToList();
            CommitteeList = FilterChoices.SelectedAskingCommittee.ToList();

            // create strings from those lists
            SelectedAskingMPsText = CreateTextGivenListMPs(SelectedAskingMPsList);
            SelectedAnsweringMPsText = CreateTextGivenListMPs(SelectedAnsweringMPsList);
            SelectedAskingMyMPsText = CreateTextGivenListMPs(SelectedAskingMyMPsList);
            SelectedAnsweringMyMPsText = CreateTextGivenListMPs(SelectedAnsweringMyMPsList);
            PublicAuthoritiesText = CreateTextGivenListPAs(PublicAuthoritiesList);
            OtherRightToAskUserText = CreateTextGivenListEntities(OtherRightToAskUserList);
            CommitteeText = CreateTextGivenListCommittees(CommitteeList);
        }

        public string CreateTextGivenListMPs(List<MP> mpList)
        {
            string text = "";
            for (int i = 0; i < mpList.Count; i++)
            {
                text += mpList[i].ShortestName;
                if (i == mpList.Count - 1)
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

        public string CreateTextGivenListPAs(List<Authority> paList)
        {
            string text = "";
            for (int i = 0; i < paList.Count; i++)
            {
                text += paList[i].ShortestName;
                if (i == paList.Count - 1)
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

        public string CreateTextGivenListEntities(List<Entity> userList)
        {
            string text = "";
            for (int i = 0; i < userList.Count; i++)
            {
                text += userList[i].ShortestName;
                if (i == userList.Count - 1)
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

        private async void EditSelectedAnsweringMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAnsweringMPsExploringPage();
            }
        }

        private async void EditAuthoritiesClicked()
        {
            string message = "Choose others to add";

            var departmentExploringPage
             = new ExploringPageWithSearchAndPreSelections(App.ReadingContext.Filters.SelectedAuthorities, message);
            await App.Current.MainPage.Navigation.PushAsync(departmentExploringPage);
        }

        private async void EditOtherSelectedAnsweringMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAnsweringMPsExploringPage();
            }
        }

        private async void EditOtherSelectedAskingMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsExploringPageAsync();
            }
        }

        private async void EditSelectedAskingMPsClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAskingMPsExploringPage();
            }
        }

        private async void ApplyFiltersAndSearch()
        {
            // TODO apply filters to the list of questions
            await Shell.Current.GoToAsync(nameof(ReadingPage));
        }
    }
}

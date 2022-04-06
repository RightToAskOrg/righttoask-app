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

        public List<MP> SelectedAnsweringMyMPsList = new List<MP>();
        private string _selectedAnsweringMyMPs;
        public string SelectedAnsweringMyMPsText
        {
            get => _selectedAnsweringMyMPs;
            set => SetProperty(ref _selectedAnsweringMyMPs, value);
        }

        public List<MP> SelectedAskingMyMPsList = new List<MP>();
        private string _selectedAskingMyMPs;
        public string SelectedAskingMyMPsText
        {
            get => _selectedAskingMyMPs;
            set => SetProperty(ref _selectedAskingMyMPs, value);
        }

        public List<MP> SelectedAskingMPsList = new List<MP>();
        private string _selectedAskingMPs;
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
        }

        // commands
        public Command AnsweringMPsFilterCommand { get; }
        public Command AskingMPsFilterCommand { get; }
        public Command AnsweringAuthoritiesFilterCommand { get; }
        public Command OtherAnsweringMPsFilterCommand { get; }
        public Command OtherAskingMPsFilterCommand { get; }
        public Command RightToAskUserCommand { get; }
        public Command NotSureCommand { get; }

        // helper methods
        public void ReinitData()
        {
            // get lists of data
            SelectedAskingMPsList = FilterChoices.SelectedAskingMPs.ToList();
            SelectedAnsweringMPsList = FilterChoices.SelectedAnsweringMPs.ToList();
            SelectedAskingMyMPsList = FilterChoices.SelectedAskingMPsMine.ToList();
            SelectedAnsweringMyMPsList= FilterChoices.SelectedAnsweringMPsMine.ToList();

            // create strings from those lists
            SelectedAskingMPsText = CreateTextGivenList(SelectedAskingMPsList);
            SelectedAnsweringMPsText = CreateTextGivenList(SelectedAnsweringMPsList);
            SelectedAskingMyMPsText = CreateTextGivenList(SelectedAskingMyMPsList);
            SelectedAnsweringMyMPsText = CreateTextGivenList(SelectedAnsweringMyMPsList);

            //foreach (MP mp in SelectedAnsweringMPsList)
            //{
            //    SelectedAnsweringMPsText += mp.ShortestName;
            //}
            //for (int i = 0; i < SelectedAnsweringMPsList.Count; i++)
            //{
            //    SelectedAnsweringMPsText += SelectedAnsweringMPsList[i].ShortestName;
            //    if (i == SelectedAnsweringMPsList.Count - 1)
            //    {
            //        // no comma + space
            //        continue;
            //    }
            //    else
            //    {
            //        SelectedAnsweringMPsText += ", ";
            //    }
            //}
        }

        public string CreateTextGivenList(List<MP> mpList)
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
    }
}

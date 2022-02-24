using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class QuestionViewModel : BaseViewModel
    {
        private static QuestionViewModel? _instance;
        public static QuestionViewModel Instance => _instance ??= new QuestionViewModel();

        private Question _question = new Question();
        public Question Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }

        private bool _isNewQuestion;
        public bool IsNewQuestion
        {
            get => _isNewQuestion;
            set => SetProperty(ref _isNewQuestion, value);
        }

        private bool _isReadingOnly;
        public bool IsReadingOnly
        {
            get => _isReadingOnly;
            set => SetProperty(ref _isReadingOnly, value);
        }

        private bool _raisedByOptionSelected;
        public bool RaisedByOptionSelected
        {
            get => _raisedByOptionSelected;
            set => SetProperty(ref _raisedByOptionSelected, value);
        }

        private bool _displayFindCommitteeButton;
        public bool DisplayFindCommitteeButton
        {
            get => _displayFindCommitteeButton;
            set => SetProperty(ref _displayFindCommitteeButton, value);
        }

        private bool _displaySenatesEstimatesSection;
        public bool DisplaySenateEstimatesSection
        {
            get => _displaySenatesEstimatesSection;
            set => SetProperty(ref _displaySenatesEstimatesSection, value);
        }

        private string _senateEstimatesAppearanceText = "";
        public string SenateEstimatesAppearanceText
        {
            get => _senateEstimatesAppearanceText;
            set => SetProperty(ref _senateEstimatesAppearanceText, value);
        }

        private bool _enableMyMPShouldRaiseButton;
        public bool EnableMyMPShouldRaiseButton
        {
            get => _enableMyMPShouldRaiseButton;
            set => SetProperty(ref _enableMyMPShouldRaiseButton, value);
        }

        private bool _enableAnotherMPShouldRaiseButton;
        public bool EnableAnotherMPShouldRaiseButton
        {
            get => _enableAnotherMPShouldRaiseButton;
            set => SetProperty(ref _enableAnotherMPShouldRaiseButton, value);
        }

        private bool _showReportLabel;
        public bool ShowReportLabel
        {
            get => _showReportLabel;
            set => SetProperty(ref _showReportLabel, value);
        }

        private string _reportLabelText = "";
        public string ReportLabelText
        {
            get => _reportLabelText;
            set => SetProperty(ref _reportLabelText, value);
        }

        private string _anotherUserButtonText = "";
        public string AnotherUserButtonText
        {
            get => _anotherUserButtonText;
            set => SetProperty(ref _anotherUserButtonText, value);
        }

        private string _notSureWhoShouldRaiseButtonText = "";
        public string NotSureWhoShouldRaiseButtonText
        {
            get => _notSureWhoShouldRaiseButtonText;
            set => SetProperty(ref _notSureWhoShouldRaiseButtonText, value);
        }

        private string _selectButtonText = "";
        public string SelectButtonText
        {
            get => _selectButtonText;
            set => SetProperty(ref _selectButtonText, value);
        }

        public bool MPButtonsEnabled => ParliamentData.MPAndOtherData.IsInitialised;
        public void UpdateMPButtons()
        {
            OnPropertyChanged("MPButtonsEnabled"); // called by the UpdatableParliamentAndMPData class to update this variable in real time
        }
        public bool NeedToFindAsker => App.ReadingContext.Filters.SelectedAnsweringMPs.IsNullOrEmpty();

        public QuestionViewModel()
        {
            // set defaults
            ResetInstance();

            // commands
            RaisedOptionSelectedCommand = new Command<string>((string buttonId) => 
            {
                int buttonIdNum = 0;
                Int32.TryParse(buttonId, out buttonIdNum);
                OnButtonPressed(buttonIdNum);
            });
            ProceedToReadingPageCommand = new AsyncCommand(async() => 
            {
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            });
            // If in read-only mode, initiate a question-reading page.
            // Similarly if my MP is answering.
            // If drafting, load a question-asker page, which will then 
            // lead to a question-reading page.
            //
            // TODO also doesn't do the right thing if you've previously selected
            // someone other than your MP.  In other words, it should enforce exclusivity -
            // either your MP(s) answer it, or an authority or other MP answers it.
            // At the moment this exclusivity is not enforced.
            NavigateForwardCommand = new AsyncCommand(async () =>
            {
                if (NeedToFindAsker)
                {
                    await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
                }
                else
                {
                    App.ReadingContext.IsReadingOnly = IsReadingOnly;
                    await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
                }
            });
            OtherPublicAuthorityButtonCommand = new AsyncCommand(async () =>
            {
                var exploringPageToSearchAuthorities
                    = new ExploringPageWithSearch(App.ReadingContext.Filters.SelectedAuthorities, "Choose authorities");
                await Shell.Current.Navigation.PushAsync(exploringPageToSearchAuthorities);
            });
            // If we already know the electorates (and hence responsible MPs), go
            // straight to the Explorer page that lists them.
            // If we don't, go to the page for entering address and finding them.
            // It will pop back to here.
            AnsweredByMyMPCommand = new AsyncCommand(async () =>
            {
                await NavigationUtils.PushMyAnsweringMPsExploringPage();
            });
            AnsweredByOtherMPCommand = new AsyncCommand(async () =>
            {
                await NavigationUtils.PushAnsweringMPsExploringPage();
            });
            SelectCommitteeButtonCommand = new Command(() => 
            {
                App.ReadingContext.Filters.SelectedAskingCommittee.Add("Senate Estimates tomorrow");
                SelectButtonText = AppResources.SelectedButtonText;
            });
        }

        public Command<string> RaisedOptionSelectedCommand { get; }
        public IAsyncCommand ProceedToReadingPageCommand { get; }
        public IAsyncCommand NavigateForwardCommand { get; }
        public IAsyncCommand OtherPublicAuthorityButtonCommand { get; }
        public IAsyncCommand AnsweredByMyMPCommand { get; }
        public IAsyncCommand AnsweredByOtherMPCommand { get; }
        public Command SelectCommitteeButtonCommand { get; }

        public void ResetInstance()
        {
            // set defaults
            Question = new Question();
            IsNewQuestion = false;
            IsReadingOnly = App.ReadingContext.IsReadingOnly;
            RaisedByOptionSelected = false;
            DisplayFindCommitteeButton = true;
            DisplaySenateEstimatesSection = false;
            SenateEstimatesAppearanceText = "";
            AnotherUserButtonText = AppResources.AnotherUserButtonText;
            NotSureWhoShouldRaiseButtonText = AppResources.NotSureButtonText;
            SelectButtonText = AppResources.SelectButtonText;
            ReportLabelText = AppResources.MPDataStillInitializing;
            App.ReadingContext.DraftQuestion = Question.QuestionText;
        }

        public void OnButtonPressed(int buttonId)
        {
            RaisedByOptionSelected = true;
            switch (buttonId)
            {
                case 0:
                    OnFindCommitteeButtonClicked();
                    break;
                case 1:
                    OnMyMPRaiseButtonClicked();
                    break;
                case 2:
                    OnOtherMPRaiseButtonClicked();
                    break;
                case 3:
                    UserShouldRaiseButtonClicked();
                    break;
                case 4:
                    NotSureWhoShouldRaiseButtonClicked();
                    break;
                default:
                    break;
            }
        }

        // methods for selecting who will raise your question
        private void OnFindCommitteeButtonClicked()
        {
            DisplayFindCommitteeButton = false;
            DisplaySenateEstimatesSection = true;
            SenateEstimatesAppearanceText =
                String.Join(" ", App.ReadingContext.Filters.SelectedAuthorities)
                    + " is appearing at Senate Estimates tomorrow";
        }

        // Note that the non-waiting for this asyc method means that the rest of the page can keep
        // Executing. That shouldn't be a problem, though, because it is invisible and therefore unclickable.
        private async void OnMyMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushMyAskingMPsExploringPage();
            }
            else
            {
                RedoButtonsForUnreadableMPData();
            }
        }

        private void RedoButtonsForUnreadableMPData()
        {
            EnableMyMPShouldRaiseButton = false;
            EnableAnotherMPShouldRaiseButton = false;
            ShowReportLabel = true;
            ReportLabelText = ParliamentData.MPAndOtherData.ErrorMessage;
        }

        private void NotSureWhoShouldRaiseButtonClicked()
        {
            NotSureWhoShouldRaiseButtonText = "Not implemented yet";
        }

        // TODO: Implement an ExporingPage constructor for people.
        private void UserShouldRaiseButtonClicked()
        {
            AnotherUserButtonText = "Not Implemented Yet";
        }

        private async void OnOtherMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                await NavigationUtils.PushAskingMPsExploringPageAsync();
            }
            else
            {
                RedoButtonsForUnreadableMPData();
            }
        }

        // If we already know the electorates (and hence responsible MPs), go
        // straight to the Explorer page that lists them.
        // If we don't, go to the page for entering address and finding them.
        // It will pop back to here.
        private async void OnAnsweredByMPButtonClicked(object sender, EventArgs e)
        {
            await NavigationUtils.PushMyAnsweringMPsExploringPage();
        }

        private async void OnAnswerByOtherMPButtonClicked(object sender, EventArgs e)
        {
            await NavigationUtils.PushAnsweringMPsExploringPage();
        }
    }
}

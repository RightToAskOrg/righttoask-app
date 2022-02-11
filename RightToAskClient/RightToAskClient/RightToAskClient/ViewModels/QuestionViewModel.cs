using RightToAskClient.Models;
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
        static private QuestionViewModel _instance;
        static public QuestionViewModel Instance { get => _instance ??= new QuestionViewModel(); }

        private Question _question;
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

        private string _senateEstimatesAppearanceText;
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

        private string _reportLabelText;
        public string ReportLabelText
        {
            get => _reportLabelText;
            set => SetProperty(ref _reportLabelText, value);
        }

        private string _anotherUserButtonText;
        public string AnotherUserButtonText
        {
            get => _anotherUserButtonText;
            set => SetProperty(ref _anotherUserButtonText, value);
        }

        private string _notSureWhoShouldRaiseButtonText;
        public string NotSureWhoShouldRaiseButtonText
        {
            get => _notSureWhoShouldRaiseButtonText;
            set => SetProperty(ref _notSureWhoShouldRaiseButtonText, value);
        }

        private string _selectButtonText;
        public string SelectButtonText
        {
            get => _selectButtonText;
            set => SetProperty(ref _selectButtonText, value);
        }

        public bool MPButtonsEnabled => App.MPDataInitialized;
        public void UpdateMPButtons()
        {
            OnPropertyChanged("MPButtonsEnabled");
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
            NavigateForwardCommand = new AsyncCommand(async() => 
            {
                await Shell.Current.GoToAsync($"//{nameof(ReadingPage)}");
            });
            SelectCommitteeButtonCommand = new Command(() => 
            {
                App.ReadingContext.Filters.SelectedAskingCommittee.Add("Senate Estimates tomorrow");
                SelectButtonText = "Selected!";
            });
        }

        public Command<string> RaisedOptionSelectedCommand { get; }
        public IAsyncCommand NavigateForwardCommand { get; }
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
            AnotherUserButtonText = "Another RightToAsk User";
            NotSureWhoShouldRaiseButtonText = "Not Sure";
            SelectButtonText = "Select";
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
        private void OnMyMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushMyAskingMPsExploringPage();
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
        private async void UserShouldRaiseButtonClicked()
        {
            AnotherUserButtonText = "Not Implemented Yet";
        }

        private void OnOtherMPRaiseButtonClicked()
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushAskingMPsExploringPageAsync();
            }
            else
            {
                RedoButtonsForUnreadableMPData();
            }
        }
    }
}

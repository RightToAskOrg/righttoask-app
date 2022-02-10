using RightToAskClient.Models;
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

        private string _senateEstimatesAppearance;
        public string SenateEstimatesAppearance
        {
            get => _senateEstimatesAppearance;
            set => SetProperty(ref _senateEstimatesAppearance, value);
        }

        public bool NeedToFindAsker => App.ReadingContext.Filters.SelectedAnsweringMPs.IsNullOrEmpty();

        public QuestionViewModel()
        {
            // set defaults
            Question = new Question();
            IsNewQuestion = false;
            IsReadingOnly = App.ReadingContext.IsReadingOnly;
            RaisedByOptionSelected = false;
            DisplayFindCommitteeButton = true;
            DisplaySenateEstimatesSection = false;
            SenateEstimatesAppearance = "";

            // commands
            RaisedOptionSelectedCommand = new Command<string>((string buttonId) => 
            {
                int buttonIdNum = 0;
                Int32.TryParse(buttonId, out buttonIdNum);
                OnButtonPressed(buttonIdNum);
            });
        }

        public Command<string> RaisedOptionSelectedCommand { get; set; }

        public void OnButtonPressed(int buttonId)
        {
            RaisedByOptionSelected = true;
            switch (buttonId)
            {
                case 0:
                    OnFindCommitteeButtonClicked();
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
        }

        // methods for selecting who will raise your question
        private void OnFindCommitteeButtonClicked()
        {
            DisplayFindCommitteeButton = false;
            DisplaySenateEstimatesSection = true;
            SenateEstimatesAppearance =
                String.Join(" ", App.ReadingContext.Filters.SelectedAuthorities)
                    + " is appearing at Senate Estimates tomorrow";
        }

        private void OnSelectCommitteeButtonClicked(object sender, EventArgs e)
        {
            App.ReadingContext.Filters.SelectedAskingCommittee.Add("Senate Estimates tomorrow");
            ((Button)sender).Text = "Selected!";

        }

        // Note that the non-waiting for this asyc method means that the rest of the page can keep
        // Executing. That shouldn't be a problem, though, because it is invisible and therefore unclickable.
        private void OnMyMPRaiseButtonClicked(object sender, EventArgs e)
        {

            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushMyAskingMPsExploringPage();
            }
            else
            {
                //redoButtonsForUnreadableMPData();
            }

            QuestionViewModel.Instance.RaisedByOptionSelected = true;
        }

        //private void redoButtonsForUnreadableMPData()
        //{
        //    myMPShouldRaiseItButton.IsEnabled = false;
        //    anotherMPShouldRaiseItButton.IsEnabled = false;
        //    reportLabel.IsVisible = true;
        //    reportLabel.Text = ParliamentData.MPAndOtherData.ErrorMessage;
        //}

        //async void OnNavigateForwardButtonClicked(object sender, EventArgs e)
        //{
        //    //await Navigation.PushAsync (readingPage);
        //    await Shell.Current.GoToAsync($"//{nameof(ReadingPage)}");
        //}

        private void NotSureWhoShouldRaiseButtonClicked(object sender, EventArgs e)
        {
            ((Button)sender).Text = $"Not implemented yet";
            QuestionViewModel.Instance.RaisedByOptionSelected = true;
        }

        // TODO: Implement an ExporingPage constructor for people.
        private async void UserShouldRaiseButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.Text = $"Not implemented yet";
                QuestionViewModel.Instance.RaisedByOptionSelected = true;
            }
        }

        private async void OnOtherMPRaiseButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushAskingMPsExploringPageAsync();
            }
            else
            {
                //redoButtonsForUnreadableMPData();
            }
            QuestionViewModel.Instance.RaisedByOptionSelected = true;
        }
    }
}

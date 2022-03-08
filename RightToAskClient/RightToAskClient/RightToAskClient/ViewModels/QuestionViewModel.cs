﻿using RightToAskClient.CryptoUtils;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
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

        private string _upvoteButtonText = AppResources.UpvoteButtonText;
        public string UpvoteButtonText
        {
            get => _upvoteButtonText;
            set => SetProperty(ref _upvoteButtonText, value);
        }

        private string _saveButtonText = AppResources.SaveAnswerButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }

        public string QuestionSuggesterButtonText => QuestionViewModel.Instance.IsNewQuestion ? AppResources.EditProfileButtonText : String.Format(AppResources.ViewOtherUserProfile, QuestionViewModel.Instance.Question.QuestionSuggester);

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
            UpvoteCommand = new Command(() =>
            {
                if (Question.AlreadyUpvoted)
                {
                    Question.UpVotes--;
                    Question.AlreadyUpvoted = false;
                    UpvoteButtonText = AppResources.UpvoteButtonText;
                }
                else
                {
                    Question.UpVotes++;
                    Question.AlreadyUpvoted = true;
                    UpvoteButtonText = AppResources.UpvotedButtonText;
                }
            });
            SaveAnswerCommand = new Command(() =>
            {
                SaveButtonText = "Answer saving not implemented";
            });
            SaveQuestionCommand = new Command(() =>
            {
                SubmitNewQuestionButton_OnClicked();
            });
            QuestionSuggesterCommand = new AsyncCommand(async () =>
            {
                RegisterPage1 otherUserProfilePage = new RegisterPage1();
                await App.Current.MainPage.Navigation.PushAsync(otherUserProfilePage);
            });
            BackCommand = new AsyncCommand(async () =>
            {
                string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go back? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
                if (result == "Yes, I'm sure.")
                {
                    _ = await Shell.Current.Navigation.PopAsync();
                }
            });
        }

        public Command<string> RaisedOptionSelectedCommand { get; }
        public IAsyncCommand ProceedToReadingPageCommand { get; }
        public IAsyncCommand NavigateForwardCommand { get; }
        public IAsyncCommand OtherPublicAuthorityButtonCommand { get; }
        public IAsyncCommand AnsweredByMyMPCommand { get; }
        public IAsyncCommand AnsweredByOtherMPCommand { get; }
        public IAsyncCommand QuestionSuggesterCommand { get; }
        public IAsyncCommand BackCommand { get; }
        public Command SaveQuestionCommand { get; }
        public Command SelectCommitteeButtonCommand { get; }
        public Command UpvoteCommand { get; }
        public Command SaveAnswerCommand { get; }

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
            Question.QuestionSuggester = Preferences.Get("DisplayName", "");
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

        async void SubmitNewQuestionButton_OnClicked()
        {
            if (!App.ReadingContext.ThisParticipant.IsRegistered)
            {
                string message = "You need to make an account to publish or vote on questions";
                bool registerNow
                    = await App.Current.MainPage.DisplayAlert("Make an account?", message, "OK", "Not now");

                if (registerNow)
                {
                    // var reg = new Registration();
                    RegisterPage1 registrationPage = new RegisterPage1();
                    registrationPage.Disappearing += setSuggester;

                    Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.display_name;
                    // Commenting-out this instruction means that the person has to push
                    // the 'publish question' button again after they've registered 
                    // their account. This seems natural to me, but is worth checking
                    // with users.
                    // registrationPage.Disappearing += saveQuestion;

                    await App.Current.MainPage.Navigation.PushAsync(registrationPage);
                    //await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
                }
            }
            else
            {
                SaveQuestion();
            }
        }

        private void setSuggester(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.display_name;
        }

        private async void SaveQuestion()
        {
            // Setting QuestionSuggester may be unnecessary
            // - it may already be set correctly -
            // but is needed if the person has just registered.
            if (App.ReadingContext.ThisParticipant.IsRegistered)
            {
                Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.display_name;
                App.ReadingContext.ExistingQuestions.Insert(0, Question);
                (bool isValid, string errorMessage) successfulSubmission = await SubmitQuestionToServer();
                App.ReadingContext.DraftQuestion = "";

                if (!successfulSubmission.isValid)
                {
                    await App.Current.MainPage.DisplayAlert("Error", successfulSubmission.errorMessage, "", "OK");
                    return;
                }

                bool goHome = await App.Current.MainPage.DisplayAlert("Question published!", "", "Home", "Write another one");

                if (goHome)
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                    //await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
                }
                else // Pop back to readingpage. TODO: fix the context so that it doesn't think you're drafting
                     // a question.  Possibly the right thing to do is pop everything and then push a reading page.
                {
                    //await Navigation.PopAsync();
                    await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
                }
            }
        }

        private async Task<(bool isValid, string message)> SubmitQuestionToServer()
        {
            // TODO: Obviously later this uploadable question will have more of the 
            // other data. Just getting it working for now.
            NewQuestionCommand uploadableQuestion = new NewQuestionCommand()
            {
                question_text = Question.QuestionText,
            };

            // TODO Check for serialisation errors.
            ClientSignedUnparsed signedQuestion = ClientSignatureGenerationService.SignMessage(uploadableQuestion,
                    App.ReadingContext.ThisParticipant.RegistrationInfo.uid);

            Result<bool> httpResponse = await RTAClient.RegisterNewQuestion(signedQuestion);
            return RTAClient.ValidateHttpResponse(httpResponse, "Question Upload");
        }
    }
}

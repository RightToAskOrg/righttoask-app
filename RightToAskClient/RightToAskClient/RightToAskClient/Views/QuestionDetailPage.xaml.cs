using System;
using RightToAskClient.HttpClients;
using RightToAskClient.CryptoUtils;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;

namespace RightToAskClient.Views
{
    public partial class QuestionDetailPage : ContentPage
    {
        public QuestionDetailPage()
        {
            InitializeComponent();

            BindingContext = QuestionViewModel.Instance;
            Title = "Question Details";
            // this is probably where I'd start adding/swapping the implementation to using MVVM
            if (QuestionViewModel.Instance.IsNewQuestion)
            {
                UpVoteButton.IsVisible = false;
                LinkOrAnswerSegment.IsVisible = false;
                SaveAnswerButton.IsVisible = false;
                QuestionSuggesterButton.Text = "Edit your profile";
            }
            else
            {
                QuestionSuggesterButton.Text = "View " + QuestionViewModel.Instance.Question.QuestionSuggester + "'s profile";
            }
            
        }

        private void UpVoteButton_OnClicked(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.UpVotes++;
        }

        // TODO: Present the UI more nicely here - this should happen if you click on the person's 
        // name, not with a separate button.
        private async void QuestionSuggesterButton_OnClicked(object sender, EventArgs e)
        {
            RegisterPage1 otherUserProfilePage = new RegisterPage1();
            await Navigation.PushAsync(otherUserProfilePage);
            //await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
        }

        // I'm not actually sure what triggers the 'send' event here, and hence not sure
        // which of these two functions should be doing the saving.
        private void Answer_Entered(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.LinkOrAnswer = ((Editor)sender).Text;
        }

        private void SaveAnswerButton_OnClicked(object sender, EventArgs e)
        {
            ((Button)sender).Text = "Answer saving not implemented";
        }

        // TODO: Re-enable button if you choose to draft another question.
        // TODO: Think about the flow in the case where you get the popup but then cancel/back
        // the registration screen. At the moment, it will just go back and (irritatingly)
        // give you the same options.
        async void SubmitNewQuestionButton_OnClicked(object sender, EventArgs e)
        {
            if (!App.ReadingContext.ThisParticipant.IsRegistered)
            {
                string message = "You need to make an account to publish or vote on questions";
                bool registerNow
                    = await DisplayAlert("Make an account?", message, "OK", "Not now");

                if (registerNow)
                {
                    // var reg = new Registration();
                    RegisterPage1 registrationPage = new RegisterPage1();
                    registrationPage.Disappearing += setSuggester;

                    // question.QuestionSuggester = readingContext.ThisParticipant.UserName;
                    // Commenting-out this instruction means that the person has to push
                    // the 'publish question' button again after they've registered 
                    // their account. This seems natural to me, but is worth checking
                    // with users.
                    // registrationPage.Disappearing += saveQuestion;

                    await Navigation.PushAsync(registrationPage);
                    //await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
                }
            }
            else
            {
                SaveQuestion("", new EventArgs());
            }
        }

        private void setSuggester(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.QuestionSuggester = App.ReadingContext.ThisParticipant.RegistrationInfo.display_name;
        }
        private async void SaveQuestion(object sender, EventArgs e)
        {

            // Setting QuestionSuggester may be unnecessary
            // - it may already be set correctly -
            // but is needed if the person has just registered.
            if (App.ReadingContext.ThisParticipant.IsRegistered)
            {
                // question.QuestionSuggester = readingContext.ThisParticipant.UserName;
                App.ReadingContext.ExistingQuestions.Insert(0, QuestionViewModel.Instance.Question);
                submitQuestionToServer();
                App.ReadingContext.DraftQuestion = "";

            }

            bool goHome = await DisplayAlert("Question published!", "", "Home", "Write another one");

            if (goHome)
            {
                await Navigation.PopToRootAsync();
                //await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
            }
            else  // Pop back to readingpage. TODO: fix the context so that it doesn't think you're drafting
                  // a question.  Possibly the right thing to do is pop everything and then push a reading page.
            {
                //await Navigation.PopAsync();
                await Shell.Current.GoToAsync($"{nameof(ReadingPage)}");
            }
        }

        private async void submitQuestionToServer()
        {
            // TODO: Obviously later this uploadable question will have more of the 
            // other data. Just getting it working for now.
            NewQuestionCommand uploadableQuestion = new NewQuestionCommand()
            {
                question_text = QuestionViewModel.Instance.Question.QuestionText,
            };
            
            ClientSignedUnparsed signedQuestion = ClientSignatureGenerationService.SignMessage(uploadableQuestion,
                    App.ReadingContext.ThisParticipant.RegistrationInfo.uid);
            await RTAClient.RegisterNewQuestion(signedQuestion);
        }

        private void Background_Entered(object sender, EventArgs e)
        {
            // Do nothing.
        }
    }
}
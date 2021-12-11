using System;
using RightToAskClient.Models;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;

namespace RightToAskClient.Views
{
    public partial class QuestionDetailPage : ContentPage
    {
        private string linkOrAnswer;
        private Question question;
        private ReadingContext readingContext;
        public QuestionDetailPage (bool isNewQuestion, Question question, ReadingContext readingContext)
        {
            BindingContext = question;
            this.question = question;
            
            // Note: this is never actually used, except to pass on to the account-editing page.
            this.readingContext = readingContext;
            
            InitializeComponent ();
            // QuestionDetailView.Text = question.ToString();
            
            // Different actions depending on whether it's a new question you're about to submit,
            // or an existing question you're answering, upvoting or adding links for.
            if (isNewQuestion)
            {
                UpVoteButton.IsVisible = false;
                LinkOrAnswerSegment.IsVisible = false;
                SaveAnswerButton.IsVisible = false;
                QuestionSuggesterButton.Text = "Edit your profile";
            }
            else
            {
                BackgroundSegment.IsVisible = false;
                SaveBackgroundButton.IsVisible = false;
                QuestionSuggesterButton.Text = "View " + question.QuestionSuggester + "'s profile";
            }
            
        }
        
        private void UpVoteButton_OnClicked(object sender, EventArgs e)
        {
            question.UpVotes++;
        }

        // TODO: Present the UI more nicely here - this should happen if you click on the person's 
        // name, not with a separate button.
        private async void QuestionSuggesterButton_OnClicked(object sender, EventArgs e)
        {
            var testUserReg = new Registration()
            {
                uid = "This is a test user",
                display_name = "testing user",
                public_key = "123",
                State = "VIC"
            };
            RegisterPage1 otherUserProfilePage = new RegisterPage1(testUserReg, readingContext, true);
            await Navigation.PushAsync(otherUserProfilePage);
        }
        
        // I'm not actually sure what triggers the 'send' event here, and hence not sure
        // which of these two functions should be doing the saving.
		void Answer_Entered(object sender, EventArgs e)
		{
			question.LinkOrAnswer = ((Editor) sender).Text;
		}

        private void SaveAnswerButton_OnClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = "Answer saving not implemented";
        }

        // TODO: Re-enable button if you choose to draft another question.
        // TODO: Think about the flow in the case where you get the popup but then cancel/back
        // the registration screen. At the moment, it will just go back and (irritatingly)
        // give you the same options.
        async void SubmitNewQuestionButton_OnClicked(object sender, EventArgs e)
        {
            if (!readingContext.ThisParticipant.Is_Registered)
            {
                string message = "You need to make an account to publish or vote on questions";
                bool registerNow 
                    = await DisplayAlert("Make an account?", message, "OK", "Not now");
                
                if (registerNow)
                {
                    // var reg = new Registration();
                    RegisterPage1 registrationPage = new RegisterPage1(readingContext.ThisParticipant.RegistrationInfo, readingContext, false);
                    registrationPage.Disappearing += setSuggester;
                    
                    // question.QuestionSuggester = readingContext.ThisParticipant.UserName;
                    // Commenting-out this instruction means that the person has to push
                    // the 'publish question' button again after they've registered 
                    // their account. This seems natural to me, but is worth checking
                    // with users.
                    // registrationPage.Disappearing += saveQuestion;
                    
                    await Navigation.PushAsync(registrationPage);
                }
            }
            else
            {
                SaveQuestion(null, null);
            }
        }

        private void setSuggester(object sender, EventArgs e)
        {
            question.QuestionSuggester = readingContext.ThisParticipant.RegistrationInfo.display_name;
        }
        private async void SaveQuestion(object sender, EventArgs e)
        {
            
            // Setting QuestionSuggester may be unnecessary
            // - it may already be set correctly -
            // but is needed if the person has just registered.
            if (readingContext.ThisParticipant.Is_Registered)
            {
                // question.QuestionSuggester = readingContext.ThisParticipant.UserName;
	            readingContext.ExistingQuestions.Insert(0, question);
                readingContext.DraftQuestion = null;
                
            }
            
            bool goHome = await DisplayAlert("Question published!", "", "Home", "Write another one");
                
            if (goHome)
            {
                await Navigation.PopToRootAsync();
            }
            else  // Pop back to readingpage. TODO: fix the context so that it doesn't think you're drafting
                // a question.  Possibly the right thing to do is pop everything and then push a reading page.
            {
                await Navigation.PopAsync();
            }
        }

        private void Background_Entered(object sender, EventArgs e)
        {
            // Do nothing.
        }
    }
}
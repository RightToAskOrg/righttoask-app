using System;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using RightToAskClient.Resx;

// This sets slightly different things to be visible depending on how we 
// arrived here.
// TODO: VT - at the moment, this is a write-up of what I'd like it to do...
// Either way, we should see:
// - the question text,
// - who should answer? Even if it's blank, it should be there

// If the question asker has been filled, or if we came via Option B,
// we should see the 'who should raise it' box (even if it's empty). If we
// came via Option A, or if we're reading someone else's question and the 
// question-asked is blank, it should be omitted.

// We should see questionWriter if it's present, even if it's this user.
// Omit if uninitialized/Anonymous.

// We should see upvotes if we're viewing a published question (including
// ours), but not if we're reviewing our own question (when it's always 0).

// Background-adding should be
// - enabled and visible for our own question (even if already published) 
// - edit-disabled but visible if present in published questions from others
// - invisible if not present in published questions from others

// Link or answer:
// - Omit when reviewing own question.
// - Include and enable when reading another person's question. (Later, we'll
//   enforce the rule that only MPs can answer and others can only add Hansard links.)
// - If this user wrote the question, and there is an answer, show the "Approve Answer"? label
//   We might want some nice icons like the thumbs-up icon on the website.

// TODO: Discuss how to express the "is follow up to" concept.

namespace RightToAskClient.Views
{
    public partial class QuestionDetailPage : ContentPage
    {
        public QuestionDetailPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            
            // Reset the updates to blank/zero so that edits can be captured.
            QuestionViewModel.Instance.ReinitQuestionUpdates();
            
            // var vm = BindingContext as QuestionViewModel;
            // vm.ReinitQuestionUpdates();
            
            QuestionViewModel.Instance.PopupLabelText = AppResources.QuestionDetailPopupText;
            
            var normalEditorStyle = App.Current.Resources["NormalEditor"] as Style;
            var disabledEditorStyle = App.Current.Resources["DisabledEditor"] as Style;
            
            if (QuestionViewModel.Instance.IsNewQuestion)
            {
                Title = AppResources.ReviewQuestionDetailsTitle;
                QuestionTextEditor.Style = normalEditorStyle;
            }
            else
            {
                Title = AppResources.QuestionDetailsTitle;
                QuestionTextEditor.Style = disabledEditorStyle;
            }

            BackgroundEditor.Style =
                QuestionViewModel.Instance.CanEditBackground ? normalEditorStyle : disabledEditorStyle;
            // Don't bother displaying it if it has no content and you can't edit it.
            BackgroundEditor.IsVisible = QuestionViewModel.Instance.CanEditBackground ||
                                         !string.IsNullOrWhiteSpace(QuestionViewModel.Instance.Question.Background);
            BackgroundLabel.IsVisible = BackgroundEditor.IsVisible;

            // Only MPs can answer questions.
            var isMP = App.ReadingContext.ThisParticipant.IsVerifiedMPAccount;
            AnswerEditor.Style = isMP ? normalEditorStyle : disabledEditorStyle;
            AnswerEditor.IsEnabled = isMP;
        }

        private void Answer_Entered(object sender, EventArgs e)
        {
            // For Hansard links:
            QuestionViewModel.Instance.NewAnswer = ((Editor)sender).Text;  
        }

        private void Link_Entered(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.NewHansardLink = ((Editor)sender).Text;
        }
        
        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage.Navigation.PopToRootAsync();
            return true;
        }

    }
}
using System;
using RightToAskClient.Models;
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
        private QuestionViewModel vm;

        // This is used only in transition from the Metadata page
        // and can be removed when it is removed.
        public QuestionDetailPage() : this(QuestionViewModel.Instance)
        {
            
        }
        public QuestionDetailPage(QuestionViewModel questionVM)
        {
            InitializeComponent();
            vm = questionVM;
            BindingContext = questionVM;
            
            // Reset the updates to blank/zero so that edits can be captured.
            questionVM.ReinitQuestionUpdates();
            
            questionVM.PopupLabelText = AppResources.QuestionDetailPopupText;
            
            var normalEditorStyle = Application.Current.Resources["NormalEditor"] as Style;
            var disabledEditorStyle = Application.Current.Resources["DisabledEditor"] as Style;
            
            

            AnswerPermissionCheckbox.IsVisible = questionVM.IsNewQuestion || questionVM.IsMyQuestion;
            AskerPermissionCheckbox.IsVisible = questionVM.IsNewQuestion || questionVM.IsMyQuestion;

            if (questionVM.IsNewQuestion)
            {
                Title = AppResources.ReviewQuestionDetailsTitle;
                QuestionTextEditor.Style = normalEditorStyle;
                AnswerCheckBox.IsChecked = true;
                RaiseCheckBox.IsChecked = true;
            }
            else
            {
                Title = AppResources.QuestionDetailsTitle;
                QuestionTextEditor.Style = disabledEditorStyle;
            }

            // You can add background to your own question.
            var backgroundBlank = string.IsNullOrWhiteSpace(questionVM.Question.Background);
            var backgroundPermission = questionVM.CanEditBackground;
            BackgroundEditor.Style = backgroundPermission && backgroundBlank ? normalEditorStyle : disabledEditorStyle;
            // Don't bother displaying it if it has no content and you can't edit it.
            BackgroundEditor.IsVisible = backgroundPermission || !backgroundBlank;
            BackgroundLabel.IsVisible = BackgroundEditor.IsVisible;

            if (questionVM.IsNewQuestion)
            {
                AnswerEditor.IsVisible = false;
                ExistingAnswers.IsVisible = false;
                AnswerLabel.IsVisible = false;
            }
            else
            {
                // Show the existing answers if there are any.
                // Only MPs can answer questions, so show the answer edit box only to them.
                var isMP = questionVM.IsVerifiedMpAccount;
                AnswerEditor.IsEnabled = isMP;
                AnswerEditor.IsVisible = isMP;
                ExistingAnswers.IsVisible = questionVM.Question.HasAnswer;
                AnswerLabel.IsVisible = AnswerEditor.IsVisible || ExistingAnswers.IsVisible;
            }
        }

        private void Answer_Entered(object sender, EventArgs e)
        {
            // For Hansard links:
            vm.NewAnswer = ((Editor)sender).Text;  
        }

        private void Link_Entered(object sender, EventArgs e)
        {
            vm.NewHansardLink = ((Editor)sender).Text;
        }

        private void LinkTapped(object sender, EventArgs e)
        {
            Console.WriteLine($">>> e {e}");
            Console.WriteLine($">>> sender {sender}");
            
        }

    }
}
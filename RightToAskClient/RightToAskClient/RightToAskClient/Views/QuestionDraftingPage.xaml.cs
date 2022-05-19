using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class SecondPage
    {
        public SecondPage()
        {
            InitializeComponent();

            // reset the question if navigating back before this page in the stack
            QuestionViewModel.Instance.ResetInstance();

            BindingContext = QuestionViewModel.Instance;
            //((QuestionViewModel)BindingContext).Page = this;
            if (App.ReadingContext.IsReadingOnly)
            {
                Title = AppResources.FindQuestionsTitle;
                QuestionViewModel.Instance.IsReadingOnly = true;
            }
            else
            {
                QuestionViewModel.Instance.IsReadingOnly = false;
                Title = AppResources.DraftMyQuestionTitle;
                QuestionViewModel.Instance.PopupLabelText = AppResources.QuestionDraftingPagePopupText;
            }
        }

        private void Question_Entered(object sender, EventArgs e)
        {
            App.ReadingContext.DraftQuestion = ((Editor)sender).Text;
            QuestionViewModel.Instance.Question.QuestionText = ((Editor)sender).Text;
        }

        protected override bool OnBackButtonPressed()
        {
            bool result = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                result = DisplayPromptBeforeNavigating();
            });
            return result;
        }

        protected bool DisplayPromptBeforeNavigating()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                string? result = await Shell.Current.DisplayActionSheet("Are you sure you want to go back? You will lose any unsaved questions.", "Cancel", "Yes, I'm sure.");
                if (result == "Yes, I'm sure.")
                {
                    _ = await Shell.Current.Navigation.PopAsync(); // pop
                }
            });
            return true; // otherwise do nothing
        }
    }
}
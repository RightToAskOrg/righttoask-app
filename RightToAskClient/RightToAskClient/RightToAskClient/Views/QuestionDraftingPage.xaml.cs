using System;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class QuestionDraftingPage
    {
        public QuestionDraftingPage()
        {
            InitializeComponent();

            // reset the question if navigating back before this page in the stack
            QuestionViewModel.Instance.ClearQuestionDataAddWriter();

            BindingContext = QuestionViewModel.Instance;
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
            var result = true;
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
                var popup = new TwoButtonPopup(QuestionViewModel.Instance, AppResources.GoHomePopupTitle, AppResources.GoHomePopupText, AppResources.CancelButtonText, AppResources.GoHomeButtonText);
                _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                if (QuestionViewModel.Instance.ApproveButtonClicked)
                {
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
                }
            });
            return true; // otherwise do nothing
        }
    }
}
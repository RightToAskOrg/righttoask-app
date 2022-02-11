using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
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
            if (App.ReadingContext.IsReadingOnly)
            {
                Title = "Find questions";
                QuestionViewModel.Instance.IsReadingOnly = true;
            }
            else
            {
                QuestionViewModel.Instance.IsReadingOnly = false;
                Title = "Direct my question";
            }
        }

        void Question_Entered(object sender, EventArgs e)
        {
            App.ReadingContext.DraftQuestion = ((Editor)sender).Text;
        }

        // If in read-only mode, initiate a question-reading page.
        // Similarly if my MP is answering.
        // If drafting, load a question-asker page, which will then 
        // lead to a question-reading page.
        //
        // TODO also doesn't do the right thing if you've previously selected
        // someone other than your MP.  In other words, it should enforce exclusivity -
        // either your MP(s) answer it, or an authority or other MP answers it.
        // At the moment this exclusivity is not enforced.
        async void OnNavigateForwardButtonClicked(object sender, EventArgs e)
        {
            //var needToFindAsker = App.ReadingContext.Filters.SelectedAnsweringMPs.IsNullOrEmpty();

            if (QuestionViewModel.Instance.NeedToFindAsker)
            {
                //await Navigation.PushAsync(questionAskerPage);
                await Shell.Current.GoToAsync($"{nameof(QuestionAskerPage)}");
            }
            else
            {
                //var readingPage = new ReadingPage(_isReadingOnly, _readingContext);
                //await Navigation.PushAsync (readingPage);
                App.ReadingContext.IsReadingOnly = QuestionViewModel.Instance.IsReadingOnly;
                await Shell.Current.GoToAsync($"//{nameof(ReadingPage)}"); // _isReadingOnly pass value
            }
        }

        async private void OnOtherPublicAuthorityButtonClicked(object sender, EventArgs e)
        {
            var exploringPageToSearchAuthorities
                = new ExploringPageWithSearch(App.ReadingContext.Filters.SelectedAuthorities, "Choose authorities");
            //await Navigation.PushAsync(exploringPageToSearchAuthorities);
            await Shell.Current.GoToAsync($"{nameof(ExploringPageWithSearch)}");
        }

        // If we already know the electorates (and hence responsible MPs), go
        // straight to the Explorer page that lists them.
        // If we don't, go to the page for entering address and finding them.
        // It will pop back to here.
        async void OnAnsweredByMPButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushMyAnsweringMPsExploringPage();
            }
            else
            {
                myMP.IsEnabled = false;
                otherMP.IsEnabled = false;
                reportLabel.IsVisible = true;
                reportLabel.Text = ParliamentData.MPAndOtherData.ErrorMessage;
            }
        }

        private async void OnAnswerByOtherMPButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushAnsweringMPsExploringPage();
            }
            else
            {
                reportLabel.IsVisible = true;
                reportLabel.Text = ParliamentData.MPAndOtherData.ErrorMessage;
            }
        }
    }
}


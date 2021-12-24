using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RightToAskClient.Controls;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        private ReadingContext readingContext;
        public QuestionAskerPage(ReadingContext readingContext)
        {
            BindingContext = readingContext;
            this.readingContext = readingContext;
            
             InitializeComponent();
            
            NavigateForwardButton.IsVisible = false;
            SenateEstimatesSelection.IsVisible = false;
            

        }

        private void OnFindCommitteeButtonClicked(object sender, EventArgs e)
        {
            ((Button) sender).IsVisible = false;
            SenateEstimatesSelection.IsVisible = true;
            SenateEstimatesAppearance.Text =
                String.Join(" ", readingContext.Filters.SelectedAuthorities)
                    + " is appearing at Senate Estimates tomorrow";
            NavigateForwardButton.IsVisible = true;
        }
        private void OnSelectCommitteeButtonClicked(object sender, EventArgs e)
        {
            readingContext.Filters.SelectedAskingCommittee.Add("Senate Estimates tomorrow");
            ((Button)sender).Text = "Selected!";
            
        }

        // Note that the non-waiting for this asyc method means that the rest of the page can keep
        // Executing. That shouldn't be a problem, though, because it is invisible and therefore unclickable.
        private void OnMyMPRaiseButtonClicked(object sender, EventArgs e)
        {
            string message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";
            
			if (ParliamentData.MPAndOtherData.IsInitialised)
			{
				var mpsExploringPage = new ExploringPage(readingContext.ThisParticipant.GroupedMPs,
					readingContext.Filters.SelectedAskingMPsMine, message);

                launchMPFindingAndSelectingPages(mpsExploringPage);
            }
            else
            {
                redoButtonsForUnreadableMPData();
            }

            NavigateForwardButton.IsVisible = true;
        }

        private void redoButtonsForUnreadableMPData()
        {
                myMPShouldRaiseItButton.IsEnabled = false;
                anotherMPShouldRaiseItButton.IsEnabled = false;
                reportLabel.IsVisible = true;
                reportLabel.Text = ParliamentData.MPAndOtherData.ErrorMessage;
        }

        private async void launchMPFindingAndSelectingPages(ExploringPage mpsExploringPage)
        {
            var nextPage = await SecondPage.ListMPsFindFirstIfNotAlreadyKnown(readingContext, mpsExploringPage, readingContext.Filters.SelectedAskingMPs);
            await Navigation.PushAsync(nextPage);
        }

        async void OnNavigateForwardButtonClicked(object sender, EventArgs e)
        {
			var readingPage = new ReadingPage(false, readingContext);
			await Navigation.PushAsync (readingPage);
        }

        private void NotSureWhoShouldRaiseButtonClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = $"Not implemented yet";	
            NavigateForwardButton.IsVisible = true;
        }

        // TODO: Implement an ExporingPage constructor for people.
        private async void UserShouldRaiseButtonClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = $"Not implemented yet";	
            NavigateForwardButton.IsVisible = true;
        }
        private async void OnOtherMPRaiseButtonClicked(object sender, EventArgs e)
        {
            string message = "Here is the complete list of MPs";
            if (ParliamentData.MPAndOtherData.IsInitialised)
			{
				var mpsExploringPage = new ExploringPageWithSearch(ParliamentData.AllMPs,
					readingContext.Filters.SelectedAskingMPs, message);

                await Navigation.PushAsync(mpsExploringPage);
            }
            else
            {
                redoButtonsForUnreadableMPData();
            }
            
            NavigateForwardButton.IsVisible = true;
        }
    }
}
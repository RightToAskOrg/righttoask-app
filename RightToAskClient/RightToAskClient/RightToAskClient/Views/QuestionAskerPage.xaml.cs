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
            
			if (ParliamentData.MPs.IsInitialised)
			{
				var mpsExploringPage = new ExploringPage(readingContext.ThisParticipant.MyMPs,
					readingContext.Filters.SelectedAskingMPsMine, message);

                launchMPFindingAndSelectingPages(mpsExploringPage);
            }
            else
            {
                myMPShouldRaiseItButton.IsEnabled = false;
                anotherMPShouldRaiseItButton.IsEnabled = false;
                reportLabel.IsVisible = true;
                reportLabel.Text = ParliamentData.MPs.ErrorMessage;
            }

            NavigateForwardButton.IsVisible = true;
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
            /*
			// var httpResponse = await App.RegItemManager.GetUsersAsync();
            var httpResponse = await RTAClient.GetUserList(); 
            
			if (String.IsNullOrEmpty(httpResponse.Err))
            {
                var selectedUsers = new ObservableCollection<string>(httpResponse.Ok);
				// listView.ItemsSource = httpResponse.Ok;
                
                var selectableUsers =
                    new ObservableCollection<Entity>(httpResponse.Ok.Select
                        (userName => new Authority() 
                            {
                                AuthorityName = userName, 
                            }
                        )
                    );
                
			ExploringPageWithSearch usersPage 
				= new ExploringPageWithSearch(selectableUsers, readingContext.Filters.SelectedAskingUsers, "Here is the complete list of MPs");
            await Navigation.PushAsync(usersPage);
            }
            else
            {
                reportLabel.Text = "Error reaching server: " + httpResponse.Err;
            }

            NavigateForwardButton.IsVisible = true;
            */
        }
        private async void OnOtherMPRaiseButtonClicked(object sender, EventArgs e)
        {
            string message = "Here is the complete list of MPs";
            if (ParliamentData.MPs.IsInitialised)
			{
				var mpsExploringPage = new ExploringPage(ParliamentData.MPs.AllMPs,
					readingContext.Filters.SelectedAskingMPs, message);

                var nextPage = await SecondPage.ListMPsFindFirstIfNotAlreadyKnown(readingContext, mpsExploringPage, readingContext.Filters.SelectedAskingMPs);
                await Navigation.PushAsync(nextPage);
            }
            else
            {
                myMPShouldRaiseItButton.IsEnabled = false;
                anotherMPShouldRaiseItButton.IsEnabled = false;
                reportLabel.IsVisible = true;
                reportLabel.Text = ParliamentData.MPs.ErrorMessage;
            }
            
            NavigateForwardButton.IsVisible = true;
        }
    }
}
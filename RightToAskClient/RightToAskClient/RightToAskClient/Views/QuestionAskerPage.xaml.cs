using System;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Controls;
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

        // TODO: at the moment this doesn't properly select the MPs-  it just lists them and lets
        // it looks like you've selected them.
        private void OnMyMPRaiseButtonClicked(object sender, EventArgs e)
        {
            string message = "These are your MPs.  Select the one(s) who should raise the question in Parliament";
            
            // TODO (Issue #9) update to use the properly-computed MPs in ThisParticipant.MyMPs
            var mpsExploringPage = new ExploringPage(readingContext.TestCurrentMPs, readingContext.Filters.SelectedAskingMPsMine, message);
			
            ListMPsFindFirstIfNotAlreadyKnown(mpsExploringPage);
            NavigateForwardButton.IsVisible = true;
        }
        
        // TODO This is a repeat of the code in SecondPage.xaml.cs. Factor out better.
        void ListMPsFindFirstIfNotAlreadyKnown(ExploringPage mpsExploringPage)
        {
            var thisParticipant = readingContext.ThisParticipant;
			
            if (! thisParticipant.MPsKnown)
            {
                var registrationPage = new RegisterPage2(thisParticipant, false, mpsExploringPage);
				
                Navigation.PushAsync(registrationPage);
            }
            else
            {
                Navigation.PushAsync(mpsExploringPage);
            }
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

        private async void UserShouldRaiseButtonClicked(object sender, EventArgs e)
        {
			var httpResponse = await App.RegItemManager.GetTasksAsync ();
            
			if (String.IsNullOrEmpty(httpResponse.Err))
            {
                var selectedUsers = new ObservableCollection<string>(httpResponse.Ok);
				// listView.ItemsSource = httpResponse.Ok;
                
                var selectableUsers =
                    new ObservableCollection<Entity>(httpResponse.Ok.Select
                        (userName => new Entity() 
                            {
                                EntityName= userName, 
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
        }
        private async void OnOtherMPRaiseButtonClicked(object sender, EventArgs e)
        {
            var allMPsAsEntities = new ObservableCollection<Entity>(BackgroundElectorateAndMPData.AllMPs); 
            ExploringPageWithSearch mpsPage 
                = new ExploringPageWithSearch(allMPsAsEntities, readingContext.Filters.SelectedAskingMPs, "Here is the complete list of MPs");
            await Navigation.PushAsync(mpsPage);
            
            NavigateForwardButton.IsVisible = true;
        } 
    }
}
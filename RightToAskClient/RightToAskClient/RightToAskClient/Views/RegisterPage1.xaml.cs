using System;
using System.Linq;
using RightToAskClient.Models;
using RightToAskClient.HttpClients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage1 : ContentPage
    {
        private ReadingContext readingContext;
        
        public RegisterPage1(Registration reg, ReadingContext readingContext, bool isReadingOnly)
        {
            InitializeComponent();
            this.readingContext = readingContext;
            BindingContext = reg;

            Title = isReadingOnly ? "User profile" : "Create Account";
            ShowTheRightButtons(reg.display_name, isReadingOnly);
            
            stateOrTerritoryPicker.ItemsSource = ParliamentData.StatesAndTerritories;
        }

        // Show and label different buttons according to whether we're registering
        // as a new user, or viewing someone else's profile.
        void ShowTheRightButtons(string name, bool isReadingOnly)
        {
	        if (isReadingOnly)
	        {
		        registerCitizenButton.IsVisible = false;
		        registerOrgButton.IsVisible = false;
		        registerMPButton.IsVisible = false;
		        doneButton.IsVisible = false;
		        
		        DMButton.Text = "Send Direct Message to " + name;
		        SeeQuestionsButton.Text = "Read questions from " + name;
		        FollowButton.Text = "Follow " + name;

	        }
	        else
	        {
		        DMButton.IsVisible = false;
		        SeeQuestionsButton.IsVisible = false;
		        FollowButton.IsVisible = false;
		        
		        if (!readingContext.ThisParticipant.MPsKnown)
		        {
			        registerCitizenButton.Text = "Next: Find your electorates";
		        }
            
		        if (!readingContext.ThisParticipant.Is_Registered)
		        {
			        registerCitizenButton.IsVisible = true;
					registerOrgButton.IsVisible = true;
					registerMPButton.IsVisible = true;
		        }
		        else
		        {
			        if (readingContext.ThisParticipant.MPsKnown)
			        {
				        DisplayAlert("Electorates already selected",
					        "You have already selected your electorates - you can change them if you like",
					        "OK");
			        }

			        registerCitizenButton.IsVisible = false;
		        }
	        }
        }
        void OnStatePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
         
            if (picker.SelectedIndex != -1)
            {
	            string state = (string)picker.SelectedItem;
	            readingContext.ThisParticipant.RegistrationInfo.State = state; 
                readingContext.ThisParticipant.UpdateChambers(state);
            }
        }
	    
        // TODO: Obviously this fake public key is only for testing.
        // We'll correct this when we decide exactly where to store the real
        // keys.
		async void OnSaveButtonClicked (object sender, EventArgs e)
		{
			var newRegistration = (Registration)BindingContext;
			newRegistration.public_key = Constants.FakePublicKey;
			var regTest = newRegistration.IsValid().Err;
			if (String.IsNullOrEmpty(regTest))
			{
				//Result<bool> httpResponse = await App.RegItemManager.SaveTaskAsync (newRegistration);
				Result<bool> httpResponse = await RTAClient.RegisterNewUser(newRegistration);
				var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
				ReportLabel.Text = httpValidation.message;
				if (httpValidation.isValid)
				{
					readingContext.ThisParticipant.RegistrationInfo = newRegistration;
					readingContext.ThisParticipant.Is_Registered = true;
					PossiblyPushElectoratesPage();
				}
			}
			else
			{
				PromptUser(regTest);
			}
		}

		private async void PromptUser(string message)
		{
			await DisplayAlert("Registration incomplete", message, "OK");
		}
		

		async void OnCancelButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
        
        // If MPs are not known, show page that allows finding electorates.
        // Whether or not they choose some, let them finish registering.
        // Make sure they've entered a name.
        async void PossiblyPushElectoratesPage()
        {
	        var currentPage = Navigation.NavigationStack.LastOrDefault();

	        if (!readingContext.ThisParticipant.MPsKnown)
	        {
		        var findElectoratesPage = new RegisterPage2(readingContext.ThisParticipant, true, false);
		        await Navigation.PushAsync(findElectoratesPage);
	        }

	        Navigation.RemovePage(currentPage);
        }

        void OnRegisterMPButtonClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = "Registering not implemented yet";
        }
        void OnRegisterOrgButtonClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = "Registering not implemented yet";
        }

        private void OnRegisterEmailFieldCompleted(object sender, EventArgs e)
        {
	        readingContext.ThisParticipant.UserEmail = ((Editor) sender).Text;
        }

        // TODO Make this put up the electorate-finding page.
        private void OnElectoratesButtonTapped(object sender, ItemTappedEventArgs e)
        {
	        return;
        }

		private void FollowButton_OnClicked(object sender, EventArgs e)
		{
			((Xamarin.Forms.Button) sender).Text = "Following not implemented";

		}

		private void DMButton_OnClicked(object sender, EventArgs e)
		{
			((Xamarin.Forms.Button) sender).Text = "DMs not implemented";
		}

		// At the moment, this pushes a brand new question-reading page,
		// which is meant to have only questions from this person, but
		// at the moment just has everything.
		// 
		// Think a bit harder about how people will navigate or understand this:
		// Will they expect to be adding a new stack layer, or popping off old ones?
		private async void SeeQuestionsButton_OnClicked(object sender, EventArgs e)
		{
			var readingPage = new ReadingPage(true, readingContext);
			await Navigation.PushAsync(readingPage);
		}
    }
}
using System;
using System.Linq;
using RightToAskClient.Models;
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
            if (!readingContext.ThisParticipant.MPsKnown)
            {
                registerCitizenButton.Text = "Next: Find your electorates";
            }
            
            if (!readingContext.ThisParticipant.Is_Registered)
            {
                registerCitizenButton.IsVisible = true;
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

		async void OnSaveButtonClicked (object sender, EventArgs e)
		{
			var newRegistration = (Registration)BindingContext;
			var regTest = newRegistration.IsValid().Err;
			if (String.IsNullOrEmpty(regTest))
			{
				Result<bool> httpResponse = await App.RegItemManager.SaveTaskAsync (newRegistration);
				var httpValidation = validateHttpResponse(httpResponse);
				reportLabel.Text = httpValidation.message;
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
		

		private (bool isValid, string message) validateHttpResponse(Result<bool> response)
		{
		if(String.IsNullOrEmpty(response.Err))
			{
				if (response.Ok)
				{
					return (true, "Server signature successfully verified.");
				}
				return (false, "Server signature verification failed");
			}
			return (false, "Server connection error" + response.Err);
		}
		async void OnCancelButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
        async void OnRegisterNameFieldCompleted(object sender, EventArgs e)
        {
        }
        
        // If MPs are not known, show page that allows finding electorates.
        // Whether or not they choose some, let them finish registering.
        // Make sure they've entered a name.
        async void PossiblyPushElectoratesPage()
        {
	        var currentPage = Navigation.NavigationStack.LastOrDefault();

	        if (!readingContext.ThisParticipant.MPsKnown)
	        {
		        var findElectoratesPage = new RegisterPage2(readingContext.ThisParticipant, true);
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
        }
    }
}
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
        public RegisterPage1(Registration reg, ReadingContext readingContext)
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
			Result<bool> httpResponse = await App.RegItemManager.SaveTaskAsync (newRegistration);
			if(String.IsNullOrEmpty(httpResponse.Err))
			{
				if (httpResponse.Ok)
				{
					reportLabel.Text = "Server signature successfully verified.";
				}
				else
				{
					reportLabel.Text = "Server signature verification failed";
				}
			}
			else
			{
				reportLabel.Text = "Server connection error" + httpResponse.Err;
			}
		}
		async void OnCancelButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
        async void OnRegisterNameFieldCompleted(object sender, EventArgs e)
        {
	        readingContext.ThisParticipant.UserName = ((Editor) sender).Text;
        }
        
        // If MPs are not known, show page that allows finding electorates.
        // Whether or not they choose some, let them finish registering.
        // Make sure they've entered a name.
        async void OnRegisterCitizenButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(readingContext.ThisParticipant.UserName))
            {
                DisplayAlert("Enter username",
                    "You need to choose a username in order to make an account",
                    "OK");
            }
            else
            {
                readingContext.ThisParticipant.Is_Registered = true;
                
                var currentPage = Navigation.NavigationStack.LastOrDefault();
                
                if (!readingContext.ThisParticipant.MPsKnown)
                {
                    var findElectoratesPage = new RegisterPage2(readingContext.ThisParticipant, true);
                    await Navigation.PushAsync(findElectoratesPage);
                }
                
                Navigation.RemovePage(currentPage);
            }
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
    }
}
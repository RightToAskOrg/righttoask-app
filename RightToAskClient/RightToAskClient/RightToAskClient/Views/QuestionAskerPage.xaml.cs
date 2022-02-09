using System;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionAskerPage : ContentPage
    {
        public QuestionAskerPage()
        {
            BindingContext = App.ReadingContext;
            InitializeComponent();

            NavigateForwardButton.IsVisible = false;
            SenateEstimatesSelection.IsVisible = false;
        }

        private void OnFindCommitteeButtonClicked(object sender, EventArgs e)
        {
            ((Button)sender).IsVisible = false;
            SenateEstimatesSelection.IsVisible = true;
            SenateEstimatesAppearance.Text =
                String.Join(" ", App.ReadingContext.Filters.SelectedAuthorities)
                    + " is appearing at Senate Estimates tomorrow";
            NavigateForwardButton.IsVisible = true;
        }
        private void OnSelectCommitteeButtonClicked(object sender, EventArgs e)
        {
            App.ReadingContext.Filters.SelectedAskingCommittee.Add("Senate Estimates tomorrow");
            ((Button)sender).Text = "Selected!";

        }

        // Note that the non-waiting for this asyc method means that the rest of the page can keep
        // Executing. That shouldn't be a problem, though, because it is invisible and therefore unclickable.
        private void OnMyMPRaiseButtonClicked(object sender, EventArgs e)
        {

            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushMyAskingMPsExploringPage();
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

        async void OnNavigateForwardButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync (readingPage);
            await Shell.Current.GoToAsync($"//{nameof(ReadingPage)}");
        }

        private void NotSureWhoShouldRaiseButtonClicked(object sender, EventArgs e)
        {
            ((Button)sender).Text = $"Not implemented yet";
            NavigateForwardButton.IsVisible = true;
        }

        // TODO: Implement an ExporingPage constructor for people.
        private async void UserShouldRaiseButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.Text = $"Not implemented yet";
                NavigateForwardButton.IsVisible = true;
            }
        }
        private async void OnOtherMPRaiseButtonClicked(object sender, EventArgs e)
        {
            if (ParliamentData.MPAndOtherData.IsInitialised)
            {
                NavigationUtils.PushAskingMPsExploringPageAsync();
            }
            else
            {
                redoButtonsForUnreadableMPData();
            }

            NavigateForwardButton.IsVisible = true;
        }
    }
}
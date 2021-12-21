using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Data;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*
 * This page allows a person to find which electorates they live in,
 * and hence which MPs represent them.
 *
 * This is used in two possible places:
 * (1) if the person clicks on 'My MP' when setting question metadata,
 * we need to know who their MPs are. After this page,
 * there will be a list of MPs loaded for them to choose from.
 * This is indicated by setting LaunchMPsSelectionPageNext to true.
 * 
 * (2) if the person tries to vote or post a question.
 * In this case, they have generated a name via RegisterPage1
 * and can skip this step (so showSkip should be set to true).
 * And we don't follow with a list of MPs for them to chose from.
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage2 : ContentPage
    {
        private Address address = new Address(); 
        
        private IndividualParticipant thisParticipant;
        private bool launchMPsSelectionPageNext;
        private ObservableCollection<MP> alreadySelectedMPs; 

        // private ParliamentData.Chamber stateLCChamber=ParliamentData.Chamber.Vic_Legislative_Council;
        // private ParliamentData.Chamber stateLAChamber=ParliamentData.Chamber.Vic_Legislative_Assembly;

        private List<string> allFederalElectorates;
        private List<string> allStateLAElectorates;
        private List<string> allStateLCElectorates;
        // alreadySelectedMPs are passed in if a Selection page is to be launched next.
        // If they're null/absent, no selection page is launched.
        public RegisterPage2(IndividualParticipant thisParticipant, bool showSkip, bool launchMPsSelectionPageNext, 
            ObservableCollection<MP>? alreadySelectedMPs = null)
        {
            InitializeComponent();
            BindingContext = thisParticipant;
            this.thisParticipant = thisParticipant;
            this.launchMPsSelectionPageNext = launchMPsSelectionPageNext;
            this.alreadySelectedMPs = alreadySelectedMPs ?? new ObservableCollection<MP>();

            KnowElectoratesFrame.IsVisible = false;
            addressSavingStack.IsVisible = false;
            
            FindMPsButton.IsVisible = false;
            if (!showSkip)
            {
                SkipButton.IsVisible = false;
            }
            
            stateOrTerritoryPicker.ItemsSource = ParliamentData.StatesAndTerritories;
            string state = thisParticipant.RegistrationInfo.State;
            stateOrTerritoryPicker.Title = String.IsNullOrEmpty(state) ? "Choose State or Territory" : state;
            stateOrTerritoryPicker.BindingContext = thisParticipant.RegistrationInfo.State;
        }
        
        void OnStatePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker != null)
            {
                int selectedIndex = picker.SelectedIndex;
         
                if (selectedIndex != -1)
                {
                    string state = (string) picker.SelectedItem;
                    thisParticipant.RegistrationInfo.State = state;
                    thisParticipant.AddSenatorsFromState(state);
                    UpdateElectoratePickerSources(state);
                }
            }
        }

        private void UpdateElectoratePickerSources(string state)
        {
            allFederalElectorates = ParliamentData.ListElectoratesInHouseOfReps(state);
            federalElectoratePicker.ItemsSource = allFederalElectorates;
            
            allStateLAElectorates 
                = ParliamentData.ListElectoratesInStateLowerHouse(state);
            stateLAElectoratePicker.ItemsSource = allStateLAElectorates;

            if (ParliamentData.HasUpperHouse(state))
            {
                allStateLCElectorates
                    = ParliamentData.ListElectoratesInStateUpperHouse(state);
                stateLCElectoratePicker.ItemsSource = allStateLCElectorates;
            }
            else
            {
                allStateLCElectorates = new List<string>();
                stateLCElectoratePicker.Title = state + " has no Upper House";
            }
        }

        void OnStateLCElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLCElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                var state = thisParticipant.RegistrationInfo.State;
                thisParticipant.AddStateUpperHouseElectorate(state, region);
                RevealNextStepIfElectoratesKnown();
            }
        }
        void OnStateLAElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLAElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                var state = thisParticipant.RegistrationInfo.State;
                thisParticipant.AddStateElectoratesGivenOneRegion(state, region);
                RevealNextStepIfElectoratesKnown();
            }    
        }

        void OnFederalElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allFederalElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                thisParticipant.AddHouseOfRepsElectorate(region);
                RevealNextStepIfElectoratesKnown();
            }
        }

        private string ChooseElectorate(Picker p, List<string> allElectorates)
        {
            int selectedIndex = p.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < allElectorates.Count)
            {
                return allElectorates[selectedIndex];
            }

            return null;

        }
        
        // TODO: Add a check that at least some electorates have been 
        // chosen, plus a prompt to remind people to fill them all
        // if only some have been chosen.
        private void RevealNextStepIfElectoratesKnown()
        {
                    FindMPsButton.IsVisible = true;
                    thisParticipant.MPsKnown = true;
        }
        
        // If we've been asked to push an MP-selecting page, go there and
        // remove this page, otherwise just pop. Note that SelectedAnsweringMPs
        // is empty/new because we didn't know this person's MPs until this page.
        private async void OnFindMPsButtonClicked(object sender, EventArgs e)
        {
            var currentPage = Navigation.NavigationStack.LastOrDefault();
        
            if (launchMPsSelectionPageNext)
            {
                string message = "These are your MPs.  Select the one(s) who should answer the question";
                  
           	    var mpsExploringPage = new ExploringPage(thisParticipant.GroupedMPs, alreadySelectedMPs , message);
                await Navigation.PushAsync(mpsExploringPage);
            }
            
            Navigation.RemovePage(currentPage); 
        }
                
        // At the moment this just chooses random electorates. 
        // TODO: We probably want this to give the person a chance to go back and fix it if wrong.
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        async void OnSubmitAddressButton_Clicked(object sender, EventArgs e)
        {
            Result<GeoscapeAddressFeature> httpResponse;
            
            string state = thisParticipant.RegistrationInfo.State;
            
            if (String.IsNullOrEmpty(state))
            {
                DisplayAlert("Please choose a state", "", "OK");
                return;
            }

            Result<bool> addressValidation = address.seemsValid();
            if (!String.IsNullOrEmpty(addressValidation.Err))
            {
                DisplayAlert(addressValidation.Err, "", "OK");
                return;
            }
            
            httpResponse = await GeoscapeClient.GetFirstAddressData(address + " " + state);
            
            if (!String.IsNullOrEmpty(httpResponse.Err))
            {
                ReportLabel.Text = httpResponse.Err;
                return;
            } 
            
            // Now we know everything is good.
            var bestAddress = httpResponse.Ok;
            AddElectorates(bestAddress);
            FindMPsButton.IsVisible = true;
            ReportLabel.Text = "";

            bool saveThisAddress = await DisplayAlert("Electorates found!", 
                // "State Assembly Electorate: "+thisParticipant.SelectedLAStateElectorate+"\n"
                // +"State Legislative Council Electorate: "+thisParticipant.SelectedLCStateElectorate+"\n"
                "Federal electorate: "+thisParticipant.CommonwealthElectorate+"\n"+
                "State lower house electorate: "+thisParticipant.StateLowerHouseElectorate+"\n"+
                "Do you want to save your address on this device? Right To Ask will not learn your address.", 
                "OK - Save address on this device", "No thanks");
            if (saveThisAddress)
            {
                SaveAddress();
            }
            
            federalElectoratePicker.TextColor = Color.Black;
            stateLAElectoratePicker.TextColor = Color.Black;
            stateLCElectoratePicker.TextColor = Color.Black;

            SkipButton.IsVisible = false;
        }

        // TODO: At the moment, this does nothing, since there's no notion of not 
        // saving the address.
        private void SaveAddress()
        {
        }

        private void AddElectorates(GeoscapeAddressFeature addressData)
        {
            thisParticipant.AddElectoratesFromGeoscapeAddress(addressData);
            thisParticipant.MPsKnown = true;
        }

        // TODO Think about what should happen if the person has made 
        // some choices, then clicks 'skip'.  At the moment, it retains 
        // the choices they made and pops the page.
        private async void OnSkipButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnStreetNumberAndNameChanged(object sender, TextChangedEventArgs e)
        {
            address.StreetNumberAndName = e.NewTextValue;
            mainScrollView.ScrollToAsync(addressSavingStack, ScrollToPosition.End, true); 
        }
        private void OnStreetNumberAndNameEntered(object sender, EventArgs e)
        {
            address.StreetNumberAndName = ((Entry)sender).Text;
        }

        private void OnCityOrSuburbChanged(object sender, TextChangedEventArgs e)
        {
            address.CityOrSuburb = e.NewTextValue;
        }
        private void OnCityOrSuburbEntered(object sender, EventArgs e)
        {
            address.CityOrSuburb =  ((Entry)sender).Text;
        }

        private void OnPostcodeChanged(object sender, TextChangedEventArgs e)
        {
            address.Postcode = e.NewTextValue;
        }
        
        private void OnPostcodeEntered(object sender, EventArgs e)
        {
            address.Postcode =  ((Entry)sender).Text;
        }

        private void KnowElectorates_Tapped(object sender, EventArgs e)
        {
            KnowElectoratesFrame.IsVisible = true;
        }

        private void LookupElectorates_Tapped(object sender, EventArgs e)
        {
            addressSavingStack.IsVisible = true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Data;
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
 * there is a list of MPs loaded for them to choose from.
 * This is implemented by inputing a page to go to next.
 * 
 * (2) if the person tries to vote or post a question.
 * In this case, they have generated a name via RegisterPage1
 * and can skip this step.  
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage2 : ContentPage
    {
        private string address;
        private IndividualParticipant thisParticipant;
        private Page nextPage;

        private BackgroundElectorateAndMPData.Chamber stateLCChamber=BackgroundElectorateAndMPData.Chamber.Vic_Legislative_Council;
        private BackgroundElectorateAndMPData.Chamber stateLAChamber=BackgroundElectorateAndMPData.Chamber.Vic_Legislative_Assembly;

        private List<string> allFederalElectorates;
        private List<string> allStateLAElectorates;
        private List<string> allStateLCElectorates;
        public RegisterPage2(IndividualParticipant thisParticipant, bool showSkip, Page nextPage = null)
        {
            InitializeComponent();
            BindingContext = thisParticipant;
            this.thisParticipant = thisParticipant;
            this.nextPage = nextPage;
            stateOrTerritoryPicker.ItemsSource = BackgroundElectorateAndMPData.StatesAndTerritories;
            
            FindMPsButton.IsVisible = false;
            if (!showSkip)
            {
                SkipButton.IsVisible = false;
            }
        }
        
        void OnStatePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
                    
            int selectedIndex = picker.SelectedIndex;
         
            if (selectedIndex != -1)
            {
                string state = (string) picker.SelectedItem;
                thisParticipant.RegistrationInfo.state = state; 
                UpdateElectoratePickerSources(state);
            }
        }

        // TODO This treats everyone as if they're VIC at the moment.
        // Add specific sources for LC and LA in specific states.
        // Also clean up repeated code.
        private void UpdateElectoratePickerSources(string state)
        {
            allFederalElectorates = BackgroundElectorateAndMPData.ListElectoratesInChamber(BackgroundElectorateAndMPData.Chamber.Australian_House_Of_Representatives);
            federalElectoratePicker.ItemsSource = allFederalElectorates;
            
            allStateLAElectorates 
                = BackgroundElectorateAndMPData.ListElectoratesInChamber(stateLAChamber);
            stateLAElectoratePicker.ItemsSource = allStateLAElectorates;
            allStateLCElectorates 
                = BackgroundElectorateAndMPData.ListElectoratesInChamber(stateLCChamber);
            stateLCElectoratePicker.ItemsSource = allStateLCElectorates;
        }

        void OnStateLCElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLCElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                thisParticipant.RegistrationInfo.AddElectorate(stateLCChamber, region);
                RevealNextStepIfElectoratesKnown();
            }
        }
        void OnStateLAElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLAElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                thisParticipant.RegistrationInfo.AddElectorate(stateLAChamber, region);
                RevealNextStepIfElectoratesKnown();
            }    
        }

        void OnFederalElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allFederalElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                thisParticipant.RegistrationInfo.AddElectorate(
                    BackgroundElectorateAndMPData.Chamber.Australian_House_Of_Representatives, region);
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
        
        // If we've been given a nextPage, go there and remove this page,
        // otherwise just pop.
        private async void OnFindMPsButtonClicked(object sender, EventArgs e)
        {
            var currentPage = Navigation.NavigationStack.LastOrDefault();
        
            if (nextPage != null)
            {
                await Navigation.PushAsync(nextPage);
            }
            
            Navigation.RemovePage(currentPage); 
        }
                
        void OnAddressEntered(object sender, EventArgs e)
        {
            address = ((Entry) sender).Text;
        }

        // At the moment this just chooses random electorates. 
        // TODO: We probably want this to give the person a chance to go back and fix it if wrong.
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        async void OnSubmitAddressButton_Clicked(object sender, EventArgs e)
        {
            GeoscapeAddressFeatureCollection addressList;
            Result<GeoscapeAddressFeatureCollection> httpResponse;
            
            if (String.IsNullOrEmpty(thisParticipant.RegistrationInfo.state))
            {
                DisplayAlert("Please choose a state", "", "OK");
                return;
            }
            
            // httpResponse = await App.RegItemManager.GetGeoscapeAddressDataAsync(AddressEntry.Text);
            
            httpResponse = await GeoscapeClientService.GetAddressDataAsync(AddressEntry.Text);
            
            // if (httpResponse == null)
            //{
            //      listView.Header = "Error reaching server. Check your Internet connection.";
            // } 
            // TODO Deal with geoscape-specific errors, possibly in the Geoscape 
            // model itself, e.g. check for the error in which the list is empty
            // and tell the person we couldn't find their address.
            if (!String.IsNullOrEmpty(httpResponse.Err))
            {
                
                addressSavingLabel.Text = "Error reaching server: "+httpResponse.Err;
                ReportLabel.Text = "Consider looking up your electorates manually";
                return;
                
            } 
            
            addressList = httpResponse.Ok;
            updateElectorates(addressList.AddressDataList);
            FindMPsButton.IsVisible = true;

            bool SaveThisAddress = await DisplayAlert("Electorates found!", 
                // "State Assembly Electorate: "+thisParticipant.SelectedLAStateElectorate+"\n"
                // +"State Legislative Council Electorate: "+thisParticipant.SelectedLCStateElectorate+"\n"
                "Federal electorate: "+thisParticipant.RegistrationInfo.CommonwealthElectorate()+"\n"+
                "State lower house electorate: "+thisParticipant.RegistrationInfo.StateLowerHouseElectorate()+"\n"+
                "Do you want to save your address on this device? Right To Ask will not learn your address.", 
                "OK - Save address on this device", "No thanks");
            if (SaveThisAddress)
            {
                SaveAddress();
            }
            
            federalElectoratePicker.TextColor = Color.Black;
            stateLAElectoratePicker.TextColor = Color.Black;
            stateLCElectoratePicker.TextColor = Color.Black;

            SkipButton.IsVisible = false;
        }

        private void updateElectorates(GeoscapeAddressFeature[] addressListAddressDataList)
        {
            if(addressListAddressDataList.Length > 0)
            {
            thisParticipant.RegistrationInfo.AddElectorate(BackgroundElectorateAndMPData.Chamber.Australian_House_Of_Representatives,  
                addressListAddressDataList[0].Properties.CommonwealthElectorate.CommElectoralName);

            // TODO: this is a bit odd because I need to check what Geoscape does for upper and lower house electorates.
            thisParticipant.RegistrationInfo.AddStateLowerHouseElectorate(thisParticipant.RegistrationInfo.state,
                addressListAddressDataList[0].Properties.StateElectorate.StateElectoralName);
            /*
            if (!allStateLAElectorates.IsNullOrEmpty())
            {
                thisParticipant.RegistrationInfo.AddElectorate(stateLAChamber,
                    allStateLAElectorates[random.Next(allStateLAElectorates.Count)]);   
            }

            if (!allStateLCElectorates.IsNullOrEmpty())
            {
                thisParticipant.RegistrationInfo.AddElectorate(stateLCChamber, 
                    allStateLCElectorates[random.Next(allStateLCElectorates.Count)]);
            }
            */
            thisParticipant.MPsKnown = true;
            }
        }

        private void SaveAddress()
        {
            thisParticipant.Address = address;
            // saveAddressButton.Text = "Address saved";
            // noSaveAddressButton.IsVisible = false;
        }

        // TODO Think about what should happen if the person has made 
        // some choices, then clicks 'skip'.  At the moment, it retains 
        // the choices they made and pops the page.
        private async void OnSkipButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

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
namespace RightToAskClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage2 : ContentPage
    {
        private string address;
        private IndividualParticipant thisParticipant;
        private Page nextPage;

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
                thisParticipant.StateOrTerritory = state; 
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
                = BackgroundElectorateAndMPData.ListElectoratesInChamber(BackgroundElectorateAndMPData.Chamber.Vic_Legislative_Assembly);
            stateLAElectoratePicker.ItemsSource = allStateLAElectorates;
            allStateLCElectorates 
                = BackgroundElectorateAndMPData.ListElectoratesInChamber(BackgroundElectorateAndMPData.Chamber.Vic_Legislative_Council);
            stateLCElectoratePicker.ItemsSource = allStateLCElectorates;
        }

        void OnStateLCElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            thisParticipant.SelectedLCStateElectorate = ChooseElectorate(picker, allStateLCElectorates);
            RevealNextStepIfElectoratesKnown();
        }
        void OnStateLAElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            thisParticipant.SelectedLAStateElectorate = ChooseElectorate(picker, allStateLAElectorates);
            RevealNextStepIfElectoratesKnown();
        }

        void OnFederalElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            thisParticipant.SelectedFederalElectorate = ChooseElectorate(picker, allFederalElectorates);
            RevealNextStepIfElectoratesKnown();

        }

        // TODO: Deal intelligently with error handling if the array index is out of bounds.
        private string ChooseElectorate(Picker p, List<string> allElectorates)
        {
            int selectedIndex = p.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < allElectorates.Count)
            {
                return allElectorates[selectedIndex];
            }

            return null;

        }
        
        private void RevealNextStepIfElectoratesKnown()
        {
            if(!String.IsNullOrEmpty(thisParticipant.SelectedLAStateElectorate)
                && !String.IsNullOrEmpty(thisParticipant.SelectedLCStateElectorate)
                && !String.IsNullOrEmpty(thisParticipant.SelectedFederalElectorate))
                {
                    FindMPsButton.IsVisible = true;
                    thisParticipant.MPsKnown = true;
                }
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
        async void OnSubmitAddressButton_Clicked(object? sender, EventArgs e)
        {
            var random = new Random();
            
            if (allFederalElectorates == null)
            {
                DisplayAlert("Please choose a state", "", "OK");
                return;
            }
                
            if (String.IsNullOrEmpty(thisParticipant.SelectedFederalElectorate))
            {
                thisParticipant.SelectedFederalElectorate 
                    = allFederalElectorates[random.Next(allFederalElectorates.Count)];
            }
            
            if(String.IsNullOrEmpty(thisParticipant.SelectedLAStateElectorate))
            {
                if (!allStateLAElectorates.IsNullOrEmpty())
                {
                    thisParticipant.SelectedLAStateElectorate 
                    = allStateLAElectorates[random.Next(allStateLAElectorates.Count)];   
                }
            }

            if (String.IsNullOrEmpty(thisParticipant.SelectedLCStateElectorate))
            {
                if (!allStateLCElectorates.IsNullOrEmpty())
                {
                    thisParticipant.SelectedLCStateElectorate 
                      = allStateLCElectorates[random.Next(allStateLCElectorates.Count)];
                }
            }

            
            thisParticipant.MPsKnown = true;

            bool SaveThisAddress = await DisplayAlert("Electorates found!", 
                "State Assembly Electorate: "+thisParticipant.SelectedLAStateElectorate+"\n"
                +"State Legislative Council Electorate: "+thisParticipant.SelectedLCStateElectorate+"\n"
                +"Federal Electorate: "+thisParticipant.SelectedFederalElectorate+"\n"
                +"Do you want to save your address on this device? Right To Ask will not learn your address.", 
                "OK - Save address on this device", "No thanks");
            if (SaveThisAddress)
            {
                SaveAddress();
            }
            
            (((Button) sender)!).IsVisible = false; 
            federalElectoratePicker.TextColor = Color.Black;
            stateLAElectoratePicker.TextColor = Color.Black;
            stateLCElectoratePicker.TextColor = Color.Black;
            ((Button) sender).IsEnabled = false;

            FindMPsButton.IsVisible = true;
            SkipButton.IsVisible = false;
        }
        
        private void SaveAddress()
        {
            thisParticipant.Address = address;
            // saveAddressButton.Text = "Address saved";
            // noSaveAddressButton.IsVisible = false;
        }

        // At the moment there is no distinction between registering and not registering,
        // except the flag set differently.
        
        private void OnNoRegisterButtonClicked(object sender, EventArgs e)
        {
            completeRegistration();
        }

        // Register both a name and electorates. At the moment, since there is no
        // public registration, this is identical to the case in which you register
        // a name and electorates.
        private void OnRegisterElectoratesButtonClicked(object sender, EventArgs e)
        {
            completeRegistration();
        }

        private void OnRegisterNameButtonClicked(object sender, EventArgs e)
        {
        }

        async private void completeRegistration()
        {
            if (!string.IsNullOrWhiteSpace(thisParticipant.UserName))
            {
                thisParticipant.Is_Registered = true;
            }
            
            // Remove page before this, which should be RegisterPage1 
            // TODO should check that this is the page we expect it to be before removing it
            // this.Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
            // This PopAsync will now go to wherever the user started registration from 
            // this.Navigation.PopAsync ();
            await Navigation.PopAsync();
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
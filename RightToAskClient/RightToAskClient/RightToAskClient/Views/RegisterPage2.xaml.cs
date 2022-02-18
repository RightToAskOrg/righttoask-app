using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
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
        FindMPsViewModel _findMPsViewModel;
        private Address _address = new Address(); 
        // private ParliamentData.Chamber stateLCChamber=ParliamentData.Chamber.Vic_Legislative_Council;
        // private ParliamentData.Chamber stateLAChamber=ParliamentData.Chamber.Vic_Legislative_Assembly;

        private List<string> _allFederalElectorates = new List<string>();
        private List<string> allStateLAElectorates = new List<string>();
        private List<string> allStateLCElectorates = new List<string>();
        // alreadySelectedMPs are passed in if a Selection page is to be launched next.
        // If they're null/absent, no selection page is launched.
        public RegisterPage2()
        {
            _findMPsViewModel = new FindMPsViewModel();
            InitializeComponent();

            KnowElectoratesFrame.IsVisible = false;
            addressSavingStack.IsVisible = false;
            
            stateOrTerritoryPicker.ItemsSource = ParliamentData.StatesAndTerritories;
            string state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
            stateOrTerritoryPicker.Title = String.IsNullOrEmpty(state) ? "Choose State or Territory" : state;
            stateOrTerritoryPicker.BindingContext = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
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
                    App.ReadingContext.ThisParticipant.RegistrationInfo.State = state;
                    App.ReadingContext.ThisParticipant.AddSenatorsFromState(state);
                    UpdateElectoratePickerSources(state);
                }
            }
        }

        private void UpdateElectoratePickerSources(string state)
        {
            _allFederalElectorates = ParliamentData.ListElectoratesInHouseOfReps(state);
            federalElectoratePicker.ItemsSource = _allFederalElectorates;
            
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
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                App.ReadingContext.ThisParticipant.AddStateUpperHouseElectorate(state, region);
                RevealNextStepIfElectoratesKnown();
            }
        }
        void OnStateLAElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLAElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                var state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;
                App.ReadingContext.ThisParticipant.AddStateElectoratesGivenOneRegion(state, region);
                RevealNextStepIfElectoratesKnown();
            }    
        }

        void OnFederalElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, _allFederalElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                App.ReadingContext.ThisParticipant.AddHouseOfRepsElectorate(region);
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

            return "";
        }
        
        // TODO: Add a check that at least some electorates have been 
        // chosen, plus a prompt to remind people to fill them all
        // if only some have been chosen.
        private void RevealNextStepIfElectoratesKnown()
        {
            _findMPsViewModel.ShowFindMPsButton = true;
            App.ReadingContext.ThisParticipant.MPsKnown = true;
        }

        private void OnStreetNumberAndNameChanged(object sender, TextChangedEventArgs e)
        {
            _address.StreetNumberAndName = e.NewTextValue;
            mainScrollView.ScrollToAsync(addressSavingStack, ScrollToPosition.End, true); 
        }
        private void OnStreetNumberAndNameEntered(object sender, EventArgs e)
        {
            _address.StreetNumberAndName = ((Entry)sender).Text;
        }

        private void OnCityOrSuburbChanged(object sender, TextChangedEventArgs e)
        {
            _address.CityOrSuburb = e.NewTextValue;
        }
        private void OnCityOrSuburbEntered(object sender, EventArgs e)
        {
            _address.CityOrSuburb =  ((Entry)sender).Text;
        }

        private void OnPostcodeChanged(object sender, TextChangedEventArgs e)
        {
            _address.Postcode = e.NewTextValue;
        }
        
        private void OnPostcodeEntered(object sender, EventArgs e)
        {
            _address.Postcode =  ((Entry)sender).Text;
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
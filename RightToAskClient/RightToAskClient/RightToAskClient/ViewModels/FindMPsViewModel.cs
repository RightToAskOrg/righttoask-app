using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class FindMPsViewModel : BaseViewModel
    {
        #region Properties
        private bool _showFindMPsButton = false;
        public bool ShowFindMPsButton
        {
            get => _showFindMPsButton;
            set => SetProperty(ref _showFindMPsButton, value);
        }
        private bool _showSkipButton = false;
        public bool ShowSkipButton
        {
            get => _showSkipButton;
            set => SetProperty(ref _showSkipButton, value);
        }

        private Address _address = new Address();

        private bool _launchMPsSelectionPageNext;
        #endregion

        // constructor
        public FindMPsViewModel()
        {
            ShowSkipButton = false;
            _launchMPsSelectionPageNext = true;

            // commands
            MPsButtonCommand = new AsyncCommand(async () =>
            {
                //var currentPage = Navigation.NavigationStack.LastOrDefault();
                if (_launchMPsSelectionPageNext)
                {
                    string message = "These are your MPs.  Select the one(s) who should answer the question";

                    var mpsExploringPage = new ExploringPage(App.ReadingContext.ThisParticipant.GroupedMPs, App.ReadingContext.Filters.SelectedAskingMPs, message);
                    await App.Current.MainPage.Navigation.PushAsync(mpsExploringPage);
                    //await Shell.Current.GoToAsync($"{nameof(ExploringPage)}");
                }
                //Navigation.RemovePage(currentPage);
            });
            SubmitAddressButtonCommand = new AsyncCommand(async () =>
            {
                OnSubmitAddressButton_Clicked();
            });
            // TODO Think about what should happen if the person has made 
            // some choices, then clicks 'skip'.  At the moment, it retains 
            // the choices they made and pops the page.
            SkipButtonCommand = new AsyncCommand(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        // commands
        public IAsyncCommand MPsButtonCommand { get; }
        public IAsyncCommand SubmitAddressButtonCommand { get; }
        public IAsyncCommand SkipButtonCommand { get; }


        // methods
        #region Methods
        // At the moment this just chooses random electorates. 
        // TODO: We probably want this to give the person a chance to go back and fix it if wrong.
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        private async void OnSubmitAddressButton_Clicked()
        {
            Result<GeoscapeAddressFeature> httpResponse;

            string state = App.ReadingContext.ThisParticipant.RegistrationInfo.State;

            if (String.IsNullOrEmpty(state))
            {
                await App.Current.MainPage.DisplayAlert("Please choose a state", "", "OK");
                return;
            }

            Result<bool> addressValidation = _address.SeemsValid();
            if (!String.IsNullOrEmpty(addressValidation.Err))
            {
                await App.Current.MainPage.DisplayAlert(addressValidation.Err, "", "OK");
                return;
            }

            httpResponse = await GeoscapeClient.GetFirstAddressData(_address + " " + state);

            if (!String.IsNullOrEmpty(httpResponse.Err))
            {
                ReportLabelText = httpResponse.Err;
                return;
            }

            // Now we know everything is good.
            var bestAddress = httpResponse.Ok;
            AddElectorates(bestAddress);
            ShowFindMPsButton = true;
            ReportLabelText = "";

            bool saveThisAddress = await App.Current.MainPage.DisplayAlert("Electorates found!",
                // "State Assembly Electorate: "+thisParticipant.SelectedLAStateElectorate+"\n"
                // +"State Legislative Council Electorate: "+thisParticipant.SelectedLCStateElectorate+"\n"
                "Federal electorate: " + App.ReadingContext.ThisParticipant.CommonwealthElectorate + "\n" +
                "State lower house electorate: " + App.ReadingContext.ThisParticipant.StateLowerHouseElectorate + "\n" +
                "Do you want to save your address on this device? Right To Ask will not learn your address.",
                "OK - Save address on this device", "No thanks");
            if (saveThisAddress)
            {
                SaveAddress();
            }
            ShowSkipButton = false;
        }

        private void AddElectorates(GeoscapeAddressFeature addressData)
        {
            App.ReadingContext.ThisParticipant.AddElectoratesFromGeoscapeAddress(addressData);
            App.ReadingContext.ThisParticipant.MPsKnown = true;
        }

        // TODO: At the moment, this does nothing, since there's no notion of not 
        // saving the address.
        private void SaveAddress()
        {
        }
        #endregion
    }
}

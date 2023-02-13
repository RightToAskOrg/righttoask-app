using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class SharingElectorateInfoViewModel : BaseViewModel
    {
        private List<string> _sharingElectorateInfoOptionValues;

        public List<string> SharingElectorateInfoOptionValues
        {
            get => _sharingElectorateInfoOptionValues;
            set => SetProperty(ref _sharingElectorateInfoOptionValues, value);
        }

        private readonly Registration? _registration;

        public IAsyncCommand BackCommand { get; }
        public IAsyncCommand SaveAndFinishCommand { get; }

        private bool _ableToFinish;

        public bool AbleToFinish
        {
            get => _ableToFinish;
            set => SetProperty(ref _ableToFinish, value);
        }

        private string _state;

        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        private string _stateOrTerritoryTitle = AppResources.StateOrTerritoryTitle;

        public string StateOrTerritoryTitle
        {
            get => _stateOrTerritoryTitle;
            set => SetProperty(ref _stateOrTerritoryTitle, value);
        }
        private string _stateOrTerritoryElectorateTitle = AppResources.StateOrTerritoryElectorateTitle;

        public string StateOrTerritoryElectorateTitle
        {
            get => _stateOrTerritoryElectorateTitle;
            set => SetProperty(ref _stateOrTerritoryElectorateTitle, value);
        }

        private string _federalElectorate;

        public string FederalElectorate
        {
            get => _federalElectorate;
            set => SetProperty(ref _federalElectorate, value);
        }

        private string _stateElectorate;

        public string StateElectorate
        {
            get => _stateElectorate;
            set => SetProperty(ref _stateElectorate, value);
        }

        private List<SharingElectorateInfoOptions> _availableOptions;

        private int _selectedIndexValue = -1;

        public int SelectedIndexValue
        {
            get => _selectedIndexValue;
            set
            {
                SetProperty(ref _selectedIndexValue, value);
                if (_registration == null)
                {
                    return;
                }

                _registration.SharingElectorateInfoOption = _availableOptions[value];
                IsStatePublic = false;
                IsFederalElectoratePublic = false;
                IsStateElectoratePublic = false;
                switch (_registration.SharingElectorateInfoOption)
                {
                    case SharingElectorateInfoOptions.StateOrTerritory:
                        IsStatePublic = true;
                        break;
                    case SharingElectorateInfoOptions.FederalElectorateAndState:
                        IsStatePublic = true;
                        IsFederalElectoratePublic = true;
                        break;
                    case SharingElectorateInfoOptions.StateElectorateAndState:
                        IsStatePublic = true;
                        IsStateElectoratePublic = true;
                        break;
                    case SharingElectorateInfoOptions.All:
                        IsStatePublic = true;
                        IsFederalElectoratePublic = true;
                        IsStateElectoratePublic = true;
                        break;
                }

                AbleToFinish = true;
            }
        }

        private bool _isStatePublic;

        public bool IsStatePublic
        {
            get => _isStatePublic;
            set => SetProperty(ref _isStatePublic, value);
        }

        private bool _isFederalElectoratePublic;

        public bool IsFederalElectoratePublic
        {
            get => _isFederalElectoratePublic;
            set => SetProperty(ref _isFederalElectoratePublic, value);
        }

        private bool _isStateElectoratePublic;

        public bool IsStateElectoratePublic
        {
            get => _isStateElectoratePublic;
            set => SetProperty(ref _isStateElectoratePublic, value);
        }

        public SharingElectorateInfoViewModel(Registration registration) : this()
        {
            _registration = registration;
            var stateElectorates = new List<string>();
            var options = SharingElectorateInfoOptions.Nothing;
            foreach (var electorate in _registration.Electorates)
            {
                switch (electorate.chamber)
                {
                    case ParliamentData.Chamber.Australian_Senate:
                        State = electorate.region;
                        options |= SharingElectorateInfoOptions.StateOrTerritory;
                        break;
                    case ParliamentData.Chamber.Australian_House_Of_Representatives:
                        FederalElectorate = electorate.region;
                        options |= SharingElectorateInfoOptions.FederalElectorate;
                        break;
                    default:
                        if (!electorate.region.IsNullOrEmpty())
                        {
                            stateElectorates.Add(electorate.region);
                            options |= SharingElectorateInfoOptions.StateElectorate;
                        }
                        break;
                }
            }
            var isTerritory = (State == ParliamentData.StateEnum.NT.ToString() || 
                               State == ParliamentData.StateEnum.ACT.ToString());
            StateOrTerritoryTitle = isTerritory ? AppResources.TerritoryTitle : AppResources.StateTitle;
            StateOrTerritoryElectorateTitle = isTerritory ? AppResources.TerritoryElectorate : AppResources.StateElectorate;

            _availableOptions = new List<SharingElectorateInfoOptions>
            {
                SharingElectorateInfoOptions.Nothing
            };
            SharingElectorateInfoOptionValues = new List<string>
            {
                AppResources.Nothing
            };

            if ((options & SharingElectorateInfoOptions.StateOrTerritory) ==
                SharingElectorateInfoOptions.StateOrTerritory)
            {
                _availableOptions.Add(SharingElectorateInfoOptions.StateOrTerritory);
                SharingElectorateInfoOptionValues.Add(StateOrTerritoryTitle);
            }

            if ((options & SharingElectorateInfoOptions.FederalElectorateAndState) ==
                SharingElectorateInfoOptions.FederalElectorateAndState)
            {
                _availableOptions.Add(SharingElectorateInfoOptions.FederalElectorateAndState);
                SharingElectorateInfoOptionValues.Add(isTerritory ? AppResources.FedElectorateTerritory : AppResources.FedElectorateState);
            }

            if ((options & SharingElectorateInfoOptions.StateElectorateAndState) ==
                SharingElectorateInfoOptions.StateElectorateAndState)
            {
                _availableOptions.Add(SharingElectorateInfoOptions.StateElectorateAndState);
                SharingElectorateInfoOptionValues.Add(isTerritory ? AppResources.TerElectorateTerritory : AppResources.StateElectorateState);
            }

            if ((options & SharingElectorateInfoOptions.All) == SharingElectorateInfoOptions.All)
            {
                _availableOptions.Add(SharingElectorateInfoOptions.All);
                SharingElectorateInfoOptionValues.Add(isTerritory ? AppResources.AllElectorateTerritory : AppResources.AllElectorateState);
            }

            if (!stateElectorates.IsNullOrEmpty())
            {
                StateElectorate = String.Join(", ", stateElectorates);
            }
        }

        public SharingElectorateInfoViewModel()
        {
            BackCommand = new AsyncCommand(async () => { await Application.Current.MainPage.Navigation.PopAsync(); });

            SaveAndFinishCommand = new AsyncCommand(async () =>
            {
                RegistrationViewModel.SaveRegistrationToPreferences(_registration);
                Debug.Assert(_registration.registrationStatus == RegistrationStatus.NotRegistered);

                _registration.public_key = ClientSignatureGenerationService.MyPublicKey;

                // Check that the registration info we're about to send to the server is valid.
                var regTest = _registration.IsValid();
                if (regTest.Success)
                {
                    await SendNewUserToServer();
                }
                // If registration info is invalid, prompt the user to fix it up.
                else
                {
                    var errorMessage = AppResources.InvalidRegistration;
                    if (regTest is ErrorResult errorResult)
                    {
                        errorMessage = errorResult.Message;
                    }

                    var popup = new OneButtonPopup(errorMessage, AppResources.OKText);
                    await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                }
            });

            State = AppResources.NoneSelected;
            FederalElectorate = AppResources.NoneSelected;
            StateElectorate = AppResources.NoneSelected;
        }

        private async Task SendNewUserToServer()
        {
            // filter value according to private information
            var httpResponse = await RTAClient.RegisterNewUser(_registration);
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            ReportLabelText = httpValidation.errorMessage;
            if (httpValidation.isValid)
            {
                _registration.registrationStatus = RegistrationStatus.Registered;
                XamarinPreferences.shared.Set(Constants.IsRegistered, _registration.IsRegistered);

                // if the response seemed successful, put it in more common terms for the user.
                if (ReportLabelText.Contains("Success"))
                {
                    ReportLabelText = AppResources.AccountCreationSuccessResponseText;
                }

                // Now we're registered, we can't change our UID - we can only update the other fields.
                Title = AppResources.EditYourAccountTitle;
                PopupLabelText = AppResources.EditAccountPopupText;
                // pop back to the QuestionDetailsPage after the account is created
                var navigation = Application.Current.MainPage.Navigation;

                // remove 3 pages
                // current - sharing electorate page
                // registration view
                // code of conduct view
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                await navigation.PopAsync();
            }
        }
    }
}
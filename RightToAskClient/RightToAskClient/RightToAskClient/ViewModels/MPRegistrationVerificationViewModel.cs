using System;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MPRegistrationVerificationViewModel : BaseViewModel
    {
        #region Properties
        
        // If the person has stated that they are an MP or staffer, return that one (there should be only one).
        // Otherwise, a new/blank one.
        private MP _mpRepresenting = new MP();
        public MP MPRepresenting
        {
            get => _mpRepresenting;
            private set => SetProperty(ref _mpRepresenting, value);
        }

        // Bool to distinguish MPs from staffers. True if MP; false if staffer.
        private bool _isStaffer;
        public bool IsStaffer
        {
            get => _isStaffer;
            set => SetProperty(ref _isStaffer, value);
        }

        // Bool to distinguish MPs from staffers. True if MP; false if staffer.
        private string _emailUsername;
        public string EmailUsername
        {
            get => _emailUsername;
            set => SetProperty(ref _emailUsername, value);
        }

        // Index of the picker for choosing email domains
        private int _parliamentaryDomainIndex = -1;
        public int ParliamentaryDomainIndex
        {
            get => _parliamentaryDomainIndex;
            set => SetProperty(ref _parliamentaryDomainIndex, value);
        }

        // The hash returned from the server that allows us to link the PIN response
        // with the initial email verification request.
        private string _mpVerificationHash = "";
        
        // The PIN entered by the user to verify that they read the email.
        private string _mpRegistrationPin;

        public string MpRegistrationPin
        {
            get => _mpRegistrationPin;
            set => SetProperty(ref _mpRegistrationPin, value);
        }

        public bool ReturnToAccountPage;

        // The domain of the email, one of 9 hardcoded valid parliamentary email domains.
        private string _domain = "";
        
        private bool _isMsgErrorShown;
        public bool IsMsgErrorShown
        {
            get => _isMsgErrorShown;
            set => SetProperty(ref _isMsgErrorShown, value);
        }
        
        private bool _isMsgSuccessShown;
        public bool IsMsgSuccessShown
        {
            get => _isMsgSuccessShown;
            set => SetProperty(ref _isMsgSuccessShown, value);
        }
        
        #endregion

        // constructor
        public MPRegistrationVerificationViewModel()
        {
            MessagingCenter.Subscribe<SelectableListViewModel, MP>(this, "ReturnToAccountPage", (sender, arg) =>
            {
                MPRepresenting = arg;
                ReturnToAccountPage = true;
                MessagingCenter.Unsubscribe<SelectableListViewModel>(this, "ReturnToAccountPage");
            });

            SendMPVerificationEmailCommand = new Command(() => { SendMPRegistrationToServer(); });
            SubmitMPRegistrationPinCommand = new AsyncCommand(async () =>
            {
                var success = await SendMPRegistrationPINToServer();
                if (success)
                {
                    StoreMPRegistration();
                    SaveMPRegistrationToPreferences();
                    MessagingCenter.Send(this, Constants.IsVerifiedMPAccount);
                    // navigate back to account page
                    if (ReturnToAccountPage)
                    {
                        await Application.Current.MainPage.Navigation.PopToRootAsync();
                    }
                }
            });
            HideErrorLayoutCommand = new Command(() => { IsMsgErrorShown = false; });
            HideSuccessLayoutCommand = new Command(() => { IsMsgSuccessShown = false; });
        }
        
        // commands
        public Command SendMPVerificationEmailCommand { get; }
        public IAsyncCommand SubmitMPRegistrationPinCommand { get; }
        
        public Command HideErrorLayoutCommand { get; }
        public Command HideSuccessLayoutCommand { get; }
        
        // methods
        public string RegisterMPReportLabelText { get; } = "";

        private async void SendMPRegistrationToServer()
        {
            _domain = _parliamentaryDomainIndex >= 0 &&
                      _parliamentaryDomainIndex < ParliamentaryURICreator.ValidParliamentaryDomains.Count
                ? ParliamentaryURICreator.ValidParliamentaryDomains[_parliamentaryDomainIndex]
                : "";
            var message = new RequestEmailValidationMessage()
            {
                why = new EmailValidationReason() { AsMP = !IsStaffer },
                name = Badge.WriteBadgeName(MPRepresenting, _domain)
            };
            var httpResponse = await RTAClient.RequestEmailValidation(
                IndividualParticipant.getInstance().SignMessage(message),
                EmailUsername + "@" + _domain);

            if(httpResponse is SuccessResult<RequestEmailValidationResponse> validResponse)
            {
                if (validResponse.Data.IsEmailSent)
                {
                    _mpVerificationHash = validResponse.Data.EmailSent;
                    ReportLabelText = AppResources.VerificationCodeSent;
                }
                else
                {
                    // Use data from validResponse.Data.AlreadyValidated;
                    ReportLabelText = AppResources.AlreadyValidated;
                }
                IsMsgErrorShown = false;
                IsMsgSuccessShown = true;
            }
            else // httpResponse is an error
            {
                var httpErrorResponse = httpResponse as ErrorResult<RequestEmailValidationResponse>;
                ReportLabelText = httpErrorResponse?.Message ?? "Error.";
                IsMsgErrorShown = true;
                IsMsgSuccessShown = false;
            }
        }

        // TODO Note it's unclear that we really need both MPRegisteredAs and RegistrationInfo.Badges - perhaps
        // cleaner to remove the former.
        private void SaveMPRegistrationToPreferences()
        {
            // Note that a staffer has booth of these flags set to true.
            XamarinPreferences.shared.Set(Constants.IsVerifiedMPAccount, true);
            XamarinPreferences.shared.Set(Constants.IsVerifiedMPStafferAccount, _isStaffer); 
            XamarinPreferences.shared.Set(Constants.MPRegisteredAs,JsonSerializer.Serialize(MPRepresenting));
            var registrationObjectToSave = new ServerUser(IndividualParticipant.getInstance().ProfileData.RegistrationInfo);
            XamarinPreferences.shared.Set(Constants.RegistrationInfo, JsonSerializer.Serialize(registrationObjectToSave));
        }

        private void StoreMPRegistration()
        {
            IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsVerifiedMPAccount = true;
            IndividualParticipant.getInstance().ProfileData.RegistrationInfo.MPRegisteredAs = MPRepresenting;
            IndividualParticipant.getInstance().ProfileData.RegistrationInfo.IsVerifiedMPStafferAccount = _isStaffer;
            IndividualParticipant.getInstance().ProfileData.RegistrationInfo.Badges.Add(
                    new Badge()
                    {
                        badge = IsStaffer ? BadgeType.MPStaff : BadgeType.MP,
                        name = Badge.WriteBadgeName(MPRepresenting, _domain)
                    });
        }

        private async Task<bool> SendMPRegistrationPINToServer()
        {
            var msg = new EmailValidationPIN()
            {
                hash = _mpVerificationHash,
                code = Int32.Parse(_mpRegistrationPin)
            };
            var httpResponse = await RTAClient.SendEmailValidationPin(
                msg,
                IndividualParticipant.getInstance().ProfileData.RegistrationInfo.uid);
            (bool isValid, string errorMsg, string hash) validation = RTAClient.ValidateHttpResponse(httpResponse, "Email Validation PIN");

                // TODO - deal properly with errors e.g. email not known.
                // also display a nice success message if applicable.
            if (!validation.isValid)
            {
                // ReportLabelText = validation.errorMsg;
                ReportLabelText = AppResources.VerifyError;
                IsMsgErrorShown = true;
                IsMsgSuccessShown = false;
            }
            else
            {
                ReportLabelText = AppResources.VerifySuccess;
                IsMsgErrorShown = false;
                IsMsgSuccessShown = true;
            }

            return validation.isValid;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MPRegistrationVerificationViewModel : BaseViewModel
    {
        #region Properties
        
        // If the person has stated that they are an MP or staffer, return that one (there should be only one).
        // Otherwise, a new/blank one.
        private MP _MPRepresenting = new MP();
        public MP MPRepresenting
        {
            get => _MPRepresenting;
            private set => SetProperty(ref _MPRepresenting, value);
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
        private string _mpRegistrationPIN = "";

        public string MPRegistrationPIN
        {
            get => _mpRegistrationPIN;
            set => SetProperty(ref _mpRegistrationPIN, value);
        }

        public bool ReturnToAccountPage = false;
        #endregion

        // constructor
        public MPRegistrationVerificationViewModel()
        {
            MessagingCenter.Subscribe<SelectableListViewModel, MP>(this, "ReturnToAccountPage", (sender, arg) =>
            {
                MPRepresenting = arg;
                ReturnToAccountPage = true;
                MessagingCenter.Unsubscribe<RegistrationViewModel>(this, "ReturnToAccountPage");
            });

            SendMPVerificationEmailCommand = new Command(() => { SendMPRegistrationToServer(); });
            SubmitMPRegistrationPINCommand = new AsyncCommand(async () =>
            {
                var success = SendMPRegistrationPINToServer();
                if (success)
                {
                    StoreMPRegistration();
                    SaveMPRegistrationToPreferences();
                }
                // navigate back to account page
                if (ReturnToAccountPage)
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                }
            });
        }
        
        // commands
        public Command SendMPVerificationEmailCommand { get; }
        public IAsyncCommand SubmitMPRegistrationPINCommand { get; }
        
        // methods
        private string _registerMPReportLabelText = "";
        public string RegisterMPReportLabelText
        {
            get => _registerMPReportLabelText;
        }

        private bool _showRegisterMPReportLabel = false;
        public bool ShowRegisterMPReportLabel
        {
            get => _showRegisterMPReportLabel;
            set => SetProperty(ref _showRegisterMPReportLabel, value);
        }
        
        // TODO
        private async void SendMPRegistrationToServer()
        {
            var domain = _parliamentaryDomainIndex >= 0  && _parliamentaryDomainIndex < ParliamentData.Domains.Count
                ? ParliamentData.Domains[_parliamentaryDomainIndex] : "";
            RequestEmailValidationMessage message = new RequestEmailValidationMessage()
            {
                why = new EmailValidationReason() { AsMP = !IsStaffer },
                name = MPRepresenting.first_name + " " + MPRepresenting.surname +" @"+domain
            };
            Result<bool> httpResponse = await RTAClient.RequestEmailValidation(message, EmailUsername + "@" + domain);
            (bool isValid, string errorMsg) validation = RTAClient.ValidateHttpResponse(httpResponse, "Email Validation Request");
            if (validation.isValid)
            {
                // TODO - show PIN entry; 
            }
            else
            {
                // TODO - deal properly with errors e.g. email not known.
                ShowRegisterMPReportLabel = true;
                ReportLabelText = validation.errorMsg;
            }
             
            Console.WriteLine("The MP registration to send to the server is "+MPRepresenting.ShortestName);
            Console.WriteLine("The email address is"+EmailUsername+"Domain: "+domain);
            Console.WriteLine("For"+(IsStaffer ? "A staffer":"The MP"));
        }

        // TODO - save to preferences.
        private void SaveMPRegistrationToPreferences()
        {
            Console.WriteLine("The MP registration to save to preferences is "+MPRepresenting.ShortestName);
        }

        // TODO
        private void StoreMPRegistration()
        {
            App.ReadingContext.ThisParticipant.IsVerifiedMPAccount = true;
            // TODO set Staffer reg too.
            App.ReadingContext.ThisParticipant.MPRegisteredAs = MPRepresenting;
            Console.WriteLine("The MP registration to store is "+MPRepresenting.ShortestName);
        }

        
        // TODO
        private bool SendMPRegistrationPINToServer()
        {
            Console.WriteLine("The PIN to send to the server is "+MPRegistrationPIN);
            return true;
        }
    }
}
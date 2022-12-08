using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509.SigI;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.Essentials;

// This class represents a person who uses the
// system and is not an MP or org representative.
namespace RightToAskClient.Models
{
	// This represents the data we need to know *only* 
	// about the user of this app, i.e. non-public data
	// in addition to the public data in 'Person'
	public class IndividualParticipant 
    {
		// Init with empty data.
		public Person ProfileData = new Person("");

		private static IndividualParticipant _instance;

		public static IndividualParticipant getInstance()
		{
			if (_instance == null)
				_instance = new IndividualParticipant();
			// _instance.Init();
			return _instance;
		}
		public bool AddressUpdated { get; set; }
		public bool HasQuestions { get; set; }
		public bool IsVerifiedMPAccount { get; set; }
		public bool IsVerifiedMPStafferAccount { get; set; }

		private MP _MPRegisteredAs = new MP();
		public MP MPRegisteredAs { 
			get => _MPRegisteredAs;
			set
			{
				_MPRegisteredAs = value;
				// FIXME - not sure why OnPropertyChanged not compiling, but anyway perhaps we want the
				// data from Registration anyway?
				// OnPropertyChanged();
				// OnPropertyChanged("RegisteredMP");
			}
		}

		// needs to be accessible on a few pages and VMs so I put it here
		public List<string> UpvotedQuestionIDs { get; set; } = new List<string>();
		public List<string> ReportedQuestionIDs { get; set; } = new List<string>();
		public List<string> RemovedQuestionIDs { get; set; } = new List<string>();

		public ClientSignedUnparsed SignMessage<T>(T message)
		{
			return ClientSignatureGenerationService.SignMessage(message, ProfileData.RegistrationInfo.uid );
		}

		public void Init()
		{
			// get account info from preferences
            var registrationPref = XamarinPreferences.shared.Get(Constants.RegistrationInfo, "");
            if (!string.IsNullOrEmpty(registrationPref))
            {
                var registrationObj = JsonSerializer.Deserialize<ServerUser>(registrationPref);
                ProfileData.RegistrationInfo 
                    = registrationObj is null ? new Registration() : new Registration(registrationObj);
                
                // We actually need to check for the stored "IsRegistered" boolean, in case they tried to
                // register but failed, for example because the server was offline.
                // So we may have stored Registration data, but not have actually succeeded in uploading it.
                if (XamarinPreferences.shared.Get(Constants.IsRegistered, false))
                {
	                ProfileData.RegistrationInfo.registrationStatus = RegistrationStatus.Registered;
                }
                else
                {
	                ProfileData.RegistrationInfo.registrationStatus = RegistrationStatus.NotRegistered;
                }
                
                
                // We have a problem if our stored registration is null but we think we registered successfully.
                Debug.Assert(registrationObj != null || ProfileData.RegistrationInfo.registrationStatus == RegistrationStatus.NotRegistered);
                
                // If we got electorates, let the app know to skip the Find My MPs step
                // TODO We may want to distinguish between ElectoratesKnown and Electorates.Any, because
                // the latter is true if only the state is known (Senate State being an electorate).
                // At the moment, this will set ElectoratesKnown, in both the app and the preferences, when the person
                // has selected their state, regardless of whether we know their electorate.
                ProfileData.RegistrationInfo.ElectoratesKnown = XamarinPreferences.shared.Get(Constants.ElectoratesKnown, false);
                
                // Retrieve MP/staffer registration. Note that staffers have both the IsVerifiedMPAccount flag and the
                // IsVerifiedMPStafferAccount flag set to true.
                IsVerifiedMPAccount = XamarinPreferences.shared.Get(Constants.IsVerifiedMPAccount, false);
                if (IsVerifiedMPAccount)
                {
                    IsVerifiedMPStafferAccount =
	                    XamarinPreferences.shared.Get(Constants.IsVerifiedMPStafferAccount, false);
                    
                    // Used when uploading an answer. 
                    var MPRepresentingjson = XamarinPreferences.shared.Get(Constants.MPRegisteredAs, "");
                    if (!string.IsNullOrEmpty(MPRepresentingjson))
                    {
                        var MPRepresenting = JsonSerializer.Deserialize<MP>(MPRepresentingjson);
                        if (MPRepresenting != null)
                        {
                            // See if we can find the registered MP in our existing list.
                            // Using field-equality operator.
                            // If so, just keep a pointer to it; if not, use a new MP object.
                            MPRegisteredAs =
                                ParliamentData.FindMPOrMakeNewOne(MPRepresenting);
                        }
                    }
                }
            }
            
            // If we already have stored a valid state, use it and set StateKnown to true.
            ProfileData.RegistrationInfo.StateKnown = false; // Should already be the default.
            var stateString =  XamarinPreferences.shared.Get(Constants.State, "");
            var state = ParliamentData.StateStringToEnum(stateString);
            if (state.Success)
            {
                ProfileData.RegistrationInfo.StateKnown = true;
                ProfileData.RegistrationInfo.SelectedStateAsEnum = state.Data;
            }

            // Use my public key from the client signature generation service.
			ProfileData.RegistrationInfo.public_key = ClientSignatureGenerationService.InitSuccessful ? ClientSignatureGenerationService.MyPublicKey : "";
		}

		public bool Validate()
        {
			var isValid = false;
			// if they are registered, they need valid registration info
            if (ProfileData.RegistrationInfo.IsRegistered)
            {
				isValid = ProfileData.Validate();
			}
			// if they are not registered, they could still have MPs known if they are in the process of creating their first question
			// before  they have the chance to create an account
            else if (ProfileData.RegistrationInfo.ElectoratesKnown)
            {
				isValid = ProfileData.RegistrationInfo.StateKnown;
            }
			return isValid;
        }
    }
} 
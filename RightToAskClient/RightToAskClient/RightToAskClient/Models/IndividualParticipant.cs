using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models.ServerCommsData;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
// This class represents a person who uses the
// system and is not an MP or org representative.
namespace RightToAskClient.Models
{
	// This represents the data we need to know *only* 
	// about the user of this app, i.e. non-public data
	// in addition to the public data in 'Person'
	public class IndividualParticipant : Person 
    {
	    private ClientSignatureGenerationService _signatureService;
            // = await ClientSignatureGenerationService.CreateClientSignatureGenerationService();
		public IndividualParticipant() 
		{
			MPsKnown = false;
			IsRegistered = false;
			initializeCryptographicKeys();
			// _signatureService = await ClientSignatureGenerationService.CreateClientSignatureGenerationService();
		}

		public bool IsRegistered { get; set; }
		public bool MPsKnown { get; set; }
		public bool AddressUpdated { get; set; }
		public bool IsVerifiedMPAccount { get; set; }
		public bool IsVerifiedMPStafferAccount { get; set; }

		private MP _MPRegisteredAs = new MP();
		public MP MPRegisteredAs { 
			get => _MPRegisteredAs;
			set
			{
				_MPRegisteredAs = value;
				OnPropertyChanged();
				OnPropertyChanged("RegisteredMP");
			}
		}

		// needs to be accessible on a few pages and VMs so I put it here
		public List<string> UpvotedQuestionIDs { get; set; } = new List<string>();
		public List<string> ReportedQuestionIDs { get; set; } = new List<string>();
		public List<string> RemovedQuestionIDs { get; set; } = new List<string>();

		// When your electorate gets updated, we automatically update your MPs.
		// TODO: Think about whether we need to check that we're not duplicating
		// inconsistent chambers/electorates.
		private void UpdateElectorate(ElectorateWithChamber knownElectorate)
		{
			RegistrationInfo.AddElectorateRemoveDuplicates(knownElectorate);
		}

		public ClientSignedUnparsed SignMessage<T>(T message)
		{
			return _signatureService.SignMessage(message, RegistrationInfo.uid );
		}

		public string MyPublicKey()
		{
			return RegistrationInfo.public_key;
		}

		private async void initializeCryptographicKeys()
		{
			_signatureService = await ClientSignatureGenerationService.CreateClientSignatureGenerationService();
			RegistrationInfo.public_key = _signatureService.MyPublicKey();
		}

		// TODO: Do some validity checking to ensure that you're not adding inconsistent
        // data, e.g. a state different from
        // the expected state.
        public void AddHouseOfRepsElectorate(string regionToAdd)
        {
	        UpdateElectorate( 
		        new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
			        regionToAdd)
	        );
        }

        // For every state except Tas, the lower house electorate determines the upper house one
        // (if there is one). For Tas, it's more complicated. So we hand this off to ParliamentData.
        public void AddStateElectoratesGivenOneRegion(string state, string regionToAdd)
        {
	        List<ElectorateWithChamber> electorates = ParliamentData.GetStateElectoratesGivenOneRegion(state, regionToAdd);
	        foreach (var e in electorates)
	        {
		       UpdateElectorate(e); 
	        }
        }

        public void AddStateUpperHouseElectorate(string state, string region)
        {
	        if (!ParliamentData.HasUpperHouse(state))
	        {	
		        Debug.WriteLine("Error: Trying to add an upper house electorate for a state that doesn't have an upper house.");
		        return;
	        }
	        Result<ParliamentData.Chamber> chamber = ParliamentData.GetUpperHouseChamber(state);
	        if (chamber.Err.IsNullOrEmpty())
	        {
				UpdateElectorate( new ElectorateWithChamber(chamber.Ok, region) );
	        }
	        
        }


        public void AddSenatorsFromState(string state)
        {
	        if (ParliamentData.StatesAndTerritories.Contains(state))
	        {
				UpdateElectorate(new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, state));
	        }
        }

		public new bool Validate()
        {
			bool isValid = false;
			// if they are registered, they need valid registration info
            if (IsRegistered)
            {
				bool validSuper = base.Validate();
                if (validSuper)
                {
					isValid = true;
				}
                else
                {
					isValid = false;
                }
            }
			// if they are not registered, they could still have MPs known if they are in the process of creating their first question
			// before  they have the chance to create an account
            else if (!IsRegistered && MPsKnown)
            {
				if (RegistrationInfo.SelectedStateAsIndex != -1)
                {
					isValid = true;
				}
                else
                {
					isValid = false;
                }
            }
			return isValid;
        }
    }
} 
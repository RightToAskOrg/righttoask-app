using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using RightToAskClient.CryptoUtils;

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

		// needs to be accessible on a few pages and VMs so I put it here
		public List<string> UpvotedQuestionIDs { get; set; } = new List<string>();
		public List<string> ReportedQuestionIDs { get; set; } = new List<string>();
		public List<string> RemovedQuestionIDs { get; set; } = new List<string>();

		private ObservableCollection<MP> _myMPs = new ObservableCollection<MP>();
		public ObservableCollection<MP> MyMPs
		{
			get
			{
				return _myMPs;
			}
			private set
			{
				UpdateMPs();
				OnPropertyChanged(nameof(MyMPs));
			}
		}

		public IEnumerable<IGrouping<ParliamentData.Chamber,MP>> GroupedMPs
		{
			get
			{
				return MyMPs.GroupBy(mp => mp.electorate.chamber);
			}
		} 

		// When your electorate gets updated, we automatically update your MPs.
		// TODO: Think about whether we need to check that we're not duplicating
		// inconsistent chambers/electorates.
		public void UpdateElectorate(ElectorateWithChamber knownElectorate)
		{
			RegistrationInfo.AddElectorateRemoveDuplicates(knownElectorate);
			UpdateMPs();
		}

		public ClientSignedUnparsed SignMessage<T>(T message)
		{
			return _signatureService.SignMessage(message, RegistrationInfo.uid );
		}

		public ClientSignedUnparsed SignMessageWithOptions<T>(T message)
		{
			return _signatureService.SignMessageWithOptions(message, RegistrationInfo.uid);
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

		// This stores a flat list of MPs, not sorted or structured by Electorate.
		// It also refreshes the MPs according to the current list of electorates so,
		// for example, if an electorate is removed, the MPs representing it will disappear.
		public void UpdateMPs()
		{
			var mps = new List<MP>();
			List<MP> mpstoadd;

			foreach (var knownElectorate in RegistrationInfo.electorates)
			{
				mpstoadd = ParliamentData.MPAndOtherData.GetMPsRepresentingElectorate(knownElectorate);
				mps.AddRange(mpstoadd);
			}

			_myMPs = new ObservableCollection<MP>(mps);
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

        public void AddElectoratesFromGeoscapeAddress(GeoscapeAddressFeature addressData)
        {
	        var commElectoralName = addressData.Properties?.CommonwealthElectorate?.CommElectoralName;
	        var region = addressData.Properties?.StateElectorate?.StateElectoralName;
	        
	        if (commElectoralName != null)
	        {
				AddHouseOfRepsElectorate(commElectoralName);
	        }

	        if (region != null)
	        {
				AddStateElectoratesGivenOneRegion(RegistrationInfo.State, region);
	        }
        }

        public void AddSenatorsFromState(string state)
        {
	        if (ParliamentData.StatesAndTerritories.Contains(state))
	        {
				UpdateElectorate(new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, state));
	        }
        }
    }
} 
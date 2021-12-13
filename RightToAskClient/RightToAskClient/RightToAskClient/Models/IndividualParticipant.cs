using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

// This class represents a person who uses the
// system and is not an MP or org representative.
namespace RightToAskClient.Models
{
	// This represents the data we need to know *only* 
	// about the user of this app, i.e. non-public data
	// in addition to the public data in 'Person'
	public class IndividualParticipant : Person 
    {
		public IndividualParticipant() : base()
		{
			MPsKnown = false;
			Is_Registered = false;
		}
		public bool Is_Registered { get; set; }
		public bool MPsKnown { get; set; }

		private ObservableCollection<MP> myMPs = new ObservableCollection<MP>();
		public ObservableCollection<MP> MyMPs
		{
			get
			{
				return myMPs;
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
		// Inserting at the beginning rather than adding at the end gives us Senators last, 
		// preceded by House of reps and, before that, state lower house.
		public void UpdateElectorate(ElectorateWithChamber knownElectorate)
		{
			registrationInfo.AddElectorateRemoveDuplicates(knownElectorate);
			UpdateMPs();
		}
		
		// This stores a flat list of MPs, not sorted or structured by Electorate.
		// It also refreshes the MPs according to the current list of electorates so,
		// for example, if an electorate is removed, the MPs representing it will disappear.
		private void UpdateMPs()
		{
			// ParliamentData.MPs.GetMPsRepresentingElectorate(FederalElectorate);
			var mps = new List<MP>();
			List<MP> mpstoadd;

			foreach (var knownElectorate in RegistrationInfo.electorates)
			{
				mpstoadd = ParliamentData.MPs.GetMPsRepresentingElectorate(knownElectorate);
				mps.AddRange(mpstoadd);
			}

			myMPs = new ObservableCollection<MP>(mps);
		}
		
        // TODO: Do some validity checking to ensure that you're not adding inconsistent
        // data, e.g. a second electorate for a given chamber, or a state different from
        // the expected state.
        public void AddHouseOfRepsElectorate(string regionToAdd)
        {
	        UpdateElectorate( 
		        new ElectorateWithChamber(ParliamentData.Chamber.Australian_House_Of_Representatives,
			        regionToAdd)
	        );
	        // registrationInfo.Electorates.Add(new ElectorateWithChamber(chamberToAdd, regionToAdd));
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
	        /*
	        Result<ParliamentData.Chamber> chamber = ParliamentData.GetLowerHouseChamber(state);
	        if (chamber.Err.IsNullOrEmpty())
	        {
				UpdateElectorate( new ElectorateWithChamber(chamber.Ok, regionToAdd) );
	        }
	        */
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
            AddHouseOfRepsElectorate(addressData.Properties.CommonwealthElectorate.CommElectoralName);

            AddStateElectoratesGivenOneRegion(RegistrationInfo.State, addressData.Properties.StateElectorate.StateElectoralName);
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
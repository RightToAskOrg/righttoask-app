using System;
using System.Collections.Generic;

// This class represents a human, who might be 
// an MP or a non-MP participant.
// It represents the public information that we might
// know about other people - use IndividualParticipant for
// data about the particular person using this app.
namespace RightToAskClient.Models
{
    public abstract class Person : Entity
    {
		private Address _address = new Address();
		private string _userEmail = "";

		private Registration _registrationInfo = new Registration();
        // TODO add attributes for a nice profile, such as a photo.

        // Initially, when we don't know the state, it's only the Australian
		// Parliament.
		protected List<ParliamentData.Chamber> ChambersRepresentedIn 
			= ParliamentData.FindChambers("");

		//protected List<ElectorateWithChamber> electorates = new List<ElectorateWithChamber>();

		public void UpdateChambers(string state)
		{
			ChambersRepresentedIn = ParliamentData.FindChambers(state);
		}

        public Registration RegistrationInfo 
        {
			get { return _registrationInfo; }
			set
			{
				_registrationInfo = value;
				OnPropertyChanged("RegistrationInfo");
			}
		}
        
        /* Many states don't have an upper house, so this just returns ""
         */
        public string StateUpperHouseElectorate
        {
	        get
	        {
				return findElectorateGivenPredicate(c => ParliamentData.IsUpperHouseChamber(c.chamber));
	        }
        }
		

        public string CommonwealthElectorate
        {
	        get
	        {
		        return findElectorateGivenPredicate(chamberPair => chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
	        }
	        
	        /*
	        var houseOfRepsElectoratePair = registrationInfo.electorates.Find(chamberPair =>
		        chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
	        if (houseOfRepsElectoratePair is null)
	        {
		        return "";
	        }

	        return houseOfRepsElectoratePair.region;
	        */
        }

        public string StateLowerHouseElectorate
        {
	        get
	        {
		        return findElectorateGivenPredicate(chamberPair => ParliamentData.IsLowerHouseChamber(chamberPair.chamber)); 
	        }
	        /*
	        var electoratePair = registrationInfo.electorates.Find(chamberPair =>
		        ParliamentData.IsLowerHouseChamber(chamberPair.chamber));
	        if (electoratePair is null)
	        {
		        return "";
	        }

	        return electoratePair.region;
	        */
        }


        private string findElectorateGivenPredicate(Predicate<ElectorateWithChamber> func)
        {
	        var electoratePair = _registrationInfo.electorates.Find(func);
	        if (electoratePair is null)
	        {
		        return "";
	        }

	        return electoratePair.region;
        }

        /*
        public string UpdateLCStateElectorate(ParliamentData.Chamber chamber, string region)
        {
		        // TODO (Issue #9) when this LC state electorate is
		        // chosen, add the representative(s) for that electorate to MyMPs.
		        // Note that you'll need to compare both the chamber and the
		        // Electorate Name.
		        registrationInfo.AddElectorate(chamber, region);
		        // OnPropertyChanged("SelectedLCStateElectorate");
        }

        public string UpdateLAStateElectorate
        {
	        get { return selectedLAStateElectorate; }
	        set
	        {
		        // TODO (Issue #9) Update MyMPs.
		        selectedLAStateElectorate = value;
		        OnPropertyChanged("SelectedLAStateElectorate");
	        }
        }

        public string SelectedFederalElectorate
        {
	        get { return selectedFederalElectorate; }
	        set
	        {
		        // TODO (Issue #9) Update MyMPs.
		        selectedFederalElectorate = value;
		        OnPropertyChanged("SelectedFederalElectorate");
	        }
        }

*/
        public Address Address
        {
	        get { return _address; }
	        set
	        {
		        _address = value;
		        OnPropertyChanged("Address");
	        }
        }
		public string UserEmail
		{
			get { return _userEmail; }
			set
			{
				_userEmail = value;
				OnPropertyChanged("UserEmail");
			}
		}

		public override string GetName()
		{
			return _registrationInfo.display_name;
		}
    }

}
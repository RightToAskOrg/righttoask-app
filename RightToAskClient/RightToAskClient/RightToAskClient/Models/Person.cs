

// This class represents a human, who might be 
// an MP or a non-MP participant.
// It represents the public information that we might
// know about other people - use IndividualParticipant for
// data about the particular person using this app.
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Models
{
    public abstract class Person : Entity
    {
		protected Address address;
        // public string StateOrTerritory { get; set; }
        
		// protected ElectorateWithChamber FederalElectorate; 
		// protected ElectorateWithChamber LAStateElectorate;
		// protected ElectorateWithChamber LCStateElectorate;
		// protected string userName;
		protected string userEmail;

		protected Registration registrationInfo = new Registration();
        // TODO add attributes for a nice profile, such as a photo.

        // Initially, when we don't know the state, it's only the Australian
		// Parliament.
		protected List<ParliamentData.Chamber> chambersRepresentedIn 
			= ParliamentData.FindChambers("");

		//protected List<ElectorateWithChamber> electorates = new List<ElectorateWithChamber>();

		public void UpdateChambers(string state)
		{
			chambersRepresentedIn = ParliamentData.FindChambers(state);
		}

        public Registration RegistrationInfo 
        {
			get { return registrationInfo; }
			set
			{
				registrationInfo = value;
				OnPropertyChanged("RegistrationInfo");
			}
		}
        
		

        // TODO this would be a lot easier if electorates was a dictionary instead of a list of pairs
        public string CommonwealthElectorate()
        {
	        var houseOfRepsElectoratePair = registrationInfo.electorates.Find(chamberPair =>
		        chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
	        if (houseOfRepsElectoratePair is null)
	        {
		        return "";
	        }

	        return houseOfRepsElectoratePair.region;
        }

        public string StateLowerHouseElectorate()
        {
	        var electoratePair = registrationInfo.electorates.Find(chamberPair =>
		        ParliamentData.IsLowerHouseChamber(chamberPair.chamber));
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
	        get { return address; }
	        set
	        {
		        address = value;
		        OnPropertyChanged("Address");
	        }
        }
		public string UserEmail
		{
			get { return userEmail; }
			set
			{
				userEmail = value;
				OnPropertyChanged("UserEmail");
			}
		}

		public override string GetName()
		{
			return registrationInfo.display_name;
		}
    }

}
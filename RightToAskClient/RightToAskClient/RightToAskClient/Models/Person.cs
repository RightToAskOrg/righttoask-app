using System;
using System.Collections.Generic;

// This class represents a human, who might be 
// an MP or a non-MP participant.
// It represents the public information that we might
// know about other people - use IndividualParticipant for
// data about the particular person using this app.
namespace RightToAskClient.Models
{
    public class Person : Entity
    {
		private Address _address = new Address();
		private string _userEmail = "";

		public override string ShortestName
		{
			get { return RegistrationInfo.display_name; }
		}


        // Initially, when we don't know the state, it's only the Australian
		// Parliament.
		protected List<ParliamentData.Chamber> ChambersRepresentedIn 
			= ParliamentData.FindChambers("");




		private Registration _registrationInfo = new Registration();
        public Registration RegistrationInfo 
        {
			get { return _registrationInfo; }
			set
			{
				_registrationInfo = value;
				OnPropertyChanged();
			}
		}
        
        /* Many states don't have an upper house, so this just returns ""
         */
        public string StateUpperHouseElectorate
        {
	        get
	        {
				return RegistrationInfo.findElectorateGivenPredicate(c => ParliamentData.IsUpperHouseChamber(c.chamber));
	        }
        }
		

        public string CommonwealthElectorate
        {
	        get
	        {
		        return RegistrationInfo.findElectorateGivenPredicate(chamberPair => chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
	        }
        }

        public string StateLowerHouseElectorate
        {
	        get
	        {
		        return RegistrationInfo.findElectorateGivenPredicate(chamberPair => ParliamentData.IsLowerHouseChamber(chamberPair.chamber)); 
	        }
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
		        OnPropertyChanged();
	        }
        }
		public string UserEmail
		{
			get { return _userEmail; }
			set
			{
				_userEmail = value;
				OnPropertyChanged();
			}
		}

		// TODO Consider fixing this so that it looks up the user and knows its other
		// attributes.
		public Person(string user)
		{
			RegistrationInfo = new Registration() { uid = user };
		}

		protected Person()
		{
		}
		 
		public override string GetName()
		{
			return _registrationInfo.display_name;
		}
		
		public void UpdateChambers(string state)
		{
			ChambersRepresentedIn = ParliamentData.FindChambers(state);
		}
        public override bool DataEquals(object other)
        {
            var person = other as Person;
            return (person != null) && RegistrationInfo.uid == person.RegistrationInfo.uid;
        }

		public bool Validate()
        {
			return RegistrationInfo.Validate();
        }
    }

}
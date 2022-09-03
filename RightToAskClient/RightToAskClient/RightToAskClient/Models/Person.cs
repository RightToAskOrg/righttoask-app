using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models.ServerCommsData;

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
			= ParliamentData.FindFederalChambers();




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
				return ParliamentData.FindOneElectorateGivenPredicate(RegistrationInfo.Electorates.ToList(), 
					c => ParliamentData.IsUpperHouseChamber(c.chamber));
	        }
        }
		

        public string CommonwealthElectorate
        {
	        get
	        {
		        return ParliamentData.FindOneElectorateGivenPredicate(RegistrationInfo.Electorates.ToList(), 
			        chamberPair => chamberPair.chamber == ParliamentData.Chamber.Australian_House_Of_Representatives);
	        }
        }

        public string StateLowerHouseElectorate
        {
	        get
	        {
		        return ParliamentData.FindOneElectorateGivenPredicate(RegistrationInfo.Electorates.ToList(),
			        chamberPair => ParliamentData.IsLowerHouseChamber(chamberPair.chamber)); 
	        }
        }

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

		public Person(PersonID user)
		{
			if (!(user.AsRTAUser is null))
			{
				RegistrationInfo = user.AsRTAUser.RegistrationInfo;
			}
			else
			{
				RegistrationInfo = new Registration();
			}
		}

		protected Person()
		{
		}
		 
		public override string GetName()
		{
			return _registrationInfo.display_name;
		}
		
		// Call only if state is known (i.e. not the default ACT).
		public void UpdateChambers(ParliamentData.StateEnum state)
		{
			ChambersRepresentedIn = ParliamentData.FindChambers(state, true);
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
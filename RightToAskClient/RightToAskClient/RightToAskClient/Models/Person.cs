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

		public override string ShortestName => RegistrationInfo.display_name;


		// Initially, when we don't know the state, it's only the Australian
		// Parliament.
		protected List<ParliamentData.Chamber> ChambersRepresentedIn 
			= ParliamentData.FindFederalChambers();




		private Registration _registrationInfo = new Registration();
        public Registration RegistrationInfo 
        {
			get => _registrationInfo;
			set
			{
				_registrationInfo = value;
				OnPropertyChanged();
			}
		}

        public Address Address
        {
	        get => _address;
	        set
	        {
		        _address = value;
		        OnPropertyChanged();
	        }
        }

		// TODO Consider fixing this so that it looks up the user and knows its other
		// attributes.
		public Person(string user)
		{
			RegistrationInfo = new Registration() { uid = user };
		}
		
		public Person(ServerUser user)
		{
			RegistrationInfo = new Registration(user);
		}

		public Person(Registration reg)
		{
			RegistrationInfo = reg;
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

		public override string ToString()
		{
			var displayBadges = string.Join(",", RegistrationInfo.Badges);
			var addBadges = displayBadges.Any() ? "\n" + displayBadges : string.Empty;
			return RegistrationInfo.display_name + " @" + RegistrationInfo.uid + addBadges;
		}
    }

}
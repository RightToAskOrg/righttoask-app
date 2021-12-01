using System.Collections.Generic;
using System.Collections.ObjectModel;

// This class represents a person who uses the
// system and is not an MP or org representative.
namespace RightToAskClient.Models
{
    public class IndividualParticipant : Person 
    {
		public IndividualParticipant() : base()
		{
			MPsKnown = false;
			Is_Registered = false;
			MyMPs = new ObservableCollection<Entity>();
		}
		public bool Is_Registered { get; set; }
		public bool MPsKnown { get; set; }
		public ObservableCollection<Entity> MyMPs { get; set; }

    }
} 
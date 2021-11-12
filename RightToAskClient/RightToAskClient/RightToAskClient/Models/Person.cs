

// This class represents a human, who might be 
// an MP or a non-MP participant.
namespace RightToAskClient.Models
{
    public abstract class Person : Entity
    {
        public string StateOrTerritory { get; set; }
        
		// protected string userName;
		protected string userEmail;

		protected Registration _registrationInfo = new Registration();
        // TODO add attributes for a nice profile, such as a photo.
        public Registration RegistrationInfo 
        {
			get { return _registrationInfo; }
			set
			{
				_registrationInfo = value;
				OnPropertyChanged("RegistrationInfo");
			}
		}
        /*
        public string UserName
        {
			get { return userName; }
			set
			{
				userName = value;
				OnPropertyChanged("UserName");
			}
		}
		*/

		public string UserEmail
		{
			get { return userEmail; }
			set
			{
				userEmail = value;
				OnPropertyChanged("UserEmail");
			}
		}
    }

}
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
	    // private ClientSignatureGenerationService _signatureService;
            // = await ClientSignatureGenerationService.CreateClientSignatureGenerationService();
		public IndividualParticipant() 
		{
			ElectoratesKnown = false;
			IsRegistered = false;
			SigningKeyRetrieved = false;
		}

		public bool IsRegistered { get; set; }
		public bool SigningKeyRetrieved { get; set; }
		public bool ElectoratesKnown { get; set; }
		public bool AddressUpdated { get; set; }
		public bool HasQuestions { get; set; }
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

		public ClientSignedUnparsed SignMessage<T>(T message)
		{
			return ClientSignatureGenerationService.SignMessage(message, RegistrationInfo.uid );
		}

		public async Task<bool> Init()
		{
			SigningKeyRetrieved = await ClientSignatureGenerationService.Init();
			RegistrationInfo.public_key = ClientSignatureGenerationService.MyPublicKey;
			return SigningKeyRetrieved;
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
            else if (!IsRegistered && ElectoratesKnown)
            {
				if (RegistrationInfo.StateKnown)
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
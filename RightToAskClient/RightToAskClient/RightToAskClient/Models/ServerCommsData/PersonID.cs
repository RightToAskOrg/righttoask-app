using System;
using System.Text.Json.Serialization;

/* These match the data type in PersonID.
 * It is expected that only one field will be non-empty.
 * Used for sending arrays/lists of people/entitites to ask
 * and raise questions.
 * */
namespace RightToAskClient.Models.ServerCommsData
{
    public class PersonID : IEquatable<PersonID>
    {
        public PersonID(MPId mpId)
        {
            MP = mpId;
        }

        public PersonID(Authority a)
        {
            Organisation = a.AuthorityName;
        }

        public PersonID(Person user)
        {
            User = user.RegistrationInfo.uid;
        }

        public PersonID()
        {
        }

        // UserUID
        [JsonPropertyName("User")]
        public string? User { get; set; }
        
        // MPId
        [JsonPropertyName("MP")]
        public MPId? MP { get; set; }
        
        // TODO Should change this to 'Authority'? 
        [JsonPropertyName("Organisation")]
        public string? Organisation { get; set; }

        // This allows for set-based Linq list operations such as removing duplicates.
        public bool Equals(PersonID other)
        {
            // Note that null == null.
            return other.User == User
                   && other.Organisation == Organisation
                   && (MP is null && other.MP is null ||
                       MP != null && other.MP != null && MP.Equals(other.MP));
        }
    }
}
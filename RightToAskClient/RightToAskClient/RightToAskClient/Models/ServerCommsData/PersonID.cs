using System;
using System.Diagnostics;
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

        // TODO at the moment, each of these simply generates the minimal data structure with a string
        // However, we probably want to edit and make constructors that fill in the extra data, either from stored json
        // or from looking up the user online.
        public Authority? AsAuthority
        {
            get
            {
                if (String.IsNullOrWhiteSpace(User) && MP is null && !String.IsNullOrWhiteSpace(Organisation))
                {
                    Debug.Assert(Organisation != null, nameof(Organisation) + " != null");
                    // The auto analyser doesn't seem to realise that it just checked this for null.
                    // Nevertheless the assignment is guaranteed not to be null because we checked.
                    return new Authority(Organisation!);
                }

                return null;
            }   
        }
        public Person? AsRTAUser 
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(User) && MP is null && String.IsNullOrWhiteSpace(Organisation))
                {
                    // The auto analyser doesn't seem to realise that it just checked this for null.
                    // Nevertheless the assignment to uid is guaranteed not to be null because we checked.
                    Debug.Assert(User != null, nameof(User) + " != null");
                    return new Person(User!);
                }
                
                return null;
            }
        }

        public MP? AsMP
        {
            get
            {
                if (String.IsNullOrWhiteSpace(User) && MP != null && String.IsNullOrWhiteSpace(Organisation))
                {
                    return new MP(MP);
                }

                return null;
            }

        }


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
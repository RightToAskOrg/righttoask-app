using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

/* These match the data type in PersonID.
 * It is expected that only one field will be non-empty.
 * Having multiple fields allows us to read the Rust data structure
 * pub enum PersonID {
    User(UserUID),
    MP(MPId),
    Organisation(OrgID),
    Committee(CommitteeId),
 * }
 * despite C# not having parameterised enums.
 * Used for sending arrays/lists of people/entitites to ask
 * and raise questions.
 * */
namespace RightToAskClient.Models.ServerCommsData
{
    public class PersonID : IEquatable<PersonID>
    {
        // Needed for json serialisation when there are other (explicit) constructors.
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
        
        [JsonPropertyName("Committee")]
        public CommitteeInfo? Committee { get; set; }
        public PersonID(Person user)
        {
            User = user.RegistrationInfo.uid;
        }
        public PersonID(MPId mpId)
        {
            MP = mpId;
        }
        public PersonID(Authority a)
        {
            Organisation = a.AuthorityName;
        }
        public PersonID(CommitteeInfo committee)
        {
            Committee = committee;
        }

        // TODO at the moment, each of these simply generates the minimal data structure with a string
        // However, we probably want to edit and make constructors that fill in the extra data, either from stored json
        // or from looking up the user online.
        // Obviously these should be ignored for json serialisation - they don't get sent to the server, they're just
        // for internal app use.
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Person? AsRTAUser 
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(User) && MP is null && Committee is null && string.IsNullOrWhiteSpace(Organisation))
                {
                    // The auto analyser doesn't seem to realise that it just checked this for null.
                    // Nevertheless the assignment to uid is guaranteed not to be null because we checked.
                    Debug.Assert(User != null, nameof(User) + " != null");
                    return new Person(User!);
                }
                
                return null;
            }
        }
        
        

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public MP? AsMP
        {
            get
            {
                if (string.IsNullOrWhiteSpace(User) && MP != null && Committee is null && string.IsNullOrWhiteSpace(Organisation))
                {
                    return new MP(MP);
                }

                return null;
            }

        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Authority? AsAuthority
        {
            get
            {
                if (string.IsNullOrWhiteSpace(User) && MP is null && Committee is null && !string.IsNullOrWhiteSpace(Organisation))
                {
                    Debug.Assert(Organisation != null, nameof(Organisation) + " != null");
                    // The auto analyser doesn't seem to realise that it just checked this for null.
                    // Nevertheless the assignment is guaranteed not to be null because we checked.
                    return new Authority(Organisation!);
                }

                return null;
            }   
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Committee? AsCommittee
        {
            get
            {
                if (string.IsNullOrWhiteSpace(User) && MP is null && Committee != null && string.IsNullOrWhiteSpace(Organisation))
                {
                    return new Committee(Committee);
                }

                return null;
            }

        }

        // This allows for set-based Linq list operations such as removing duplicates.
        public bool Equals(PersonID? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // Note that null == null.
            return other.User == User
                   && other.Organisation == Organisation
                   && (MP is null && other.MP is null ||
                       MP != null && other.MP != null && MP.Equals(other.MP))
                   && (Committee is null && other.Committee is null ||
                       Committee != null && other.Committee != null && Committee.Equals(other.Committee));
        }
    }
}
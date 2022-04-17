using System.Text.Json.Serialization;

/* These match the data type in PersonID.
 * It is expected that only one field will be non-empty.
 * Used for sending arrays/lists of people/entitites to ask
 * and raise questions.
 * */
namespace RightToAskClient.Models.ServerCommsData
{
    public class PersonID
    {
        public PersonID(MPId mpId)
        {
            MP = mpId;
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
        
        // Add Org later
        [JsonPropertyName("Organisation")]
        public string? Organisation { get; set; }
    }
}
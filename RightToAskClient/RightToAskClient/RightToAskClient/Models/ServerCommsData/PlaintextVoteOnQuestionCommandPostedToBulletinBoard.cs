using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class PlaintextVoteOnQuestionCommandPostedToBulletinBoard
    {
        [JsonPropertyName("command")] 
        public ClientSignedUnparsed command { get; set; } 
        
        
    }
}
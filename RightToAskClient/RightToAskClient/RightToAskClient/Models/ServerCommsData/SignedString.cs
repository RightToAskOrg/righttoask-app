namespace RightToAskClient.Models.ServerCommsData
{
    // Signature is base64 encoded.
    public class SignedString
    {
        public string message { get; set; } = "";
        public string signature { get; set; } = "";
    }
}
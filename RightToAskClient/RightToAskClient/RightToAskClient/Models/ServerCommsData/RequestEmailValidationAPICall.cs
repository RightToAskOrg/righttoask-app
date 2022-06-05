namespace RightToAskClient.Models.ServerCommsData

/* For sending to the server in order to apply for email validation
 * This data structure is exactly the same as ClientSignedUnparsed, except for the
 * addition of the unsigned data (email). At the moment, it is the only such structure,
 * but there may be others later. Consider a refactor.
 */
{
    public class RequestEmailValidationAPICall
    {
        public string email;
        public string message;
        public string signature;
        public string user;
    }
}
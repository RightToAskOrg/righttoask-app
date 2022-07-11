namespace RightToAskClient.Models
{
    public class Result<T>
    {
        // TODO This is non-ideal because I can't declare a type
        // parameter to be nullable. 
        // Look at whether more recent versions of the language have nicer ways of doing this,
        // e.g. https://stackoverflow.com/questions/59702550/c-sharp-8-nullables-and-result-container
        public T Ok { get; set; }
        public string? Err { get; set; }
    }
}
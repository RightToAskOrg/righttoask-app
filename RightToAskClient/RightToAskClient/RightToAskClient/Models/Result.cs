namespace RightToAskClient.Models
{
    public class Result<T>
    {
        // TODO This is non-ideal because I can't declare a type
        // parameter to be nullable. 
        public T Ok { get; set; }
        public string? Err { get; set; }
    }
}
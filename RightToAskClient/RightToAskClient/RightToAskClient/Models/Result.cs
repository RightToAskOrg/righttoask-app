namespace RightToAskClient.Models
{
    public class Result<T>
    {
        public T Ok { get; set; } 
        public string Err { get; set; }
    }
}
using System;

public class TriedToUploadWhileNotRegisteredException : Exception
{
    public TriedToUploadWhileNotRegisteredException()
    {
    }

    public TriedToUploadWhileNotRegisteredException(string message)
        : base(message)
    {
    }

    public TriedToUploadWhileNotRegisteredException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
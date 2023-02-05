using System;

namespace RightToAskClient.Models.ServerCommsData
{
    public class ServerResult<T>
    {
        // Used only for communication with server. For use in app, use JOSResult.
        public T Ok { get; set; }
        public string? Err { get; set; }

        // Converts a ServerResult into the appropriate JOS Result
        // If there are successive layers or errors, it takes only the first one.
        // Works only when the nested Results all have the same type parameter. 
        public JOSResult<T> ToJOSResult()
        {
            if (Err is null)
            {
                if (Ok is null)
                {
                    throw new Exception("Can't make a Data-full JOSResult with no data. Use plain JOSResult instead.");
                }

                // Recurse for nested results.
                // This will produce an Ok (with data) if there are Oks all the way down.
                if (Ok is ServerResult<T> serverResult)
                {
                    return serverResult.ToJOSResult();
                }
                
                return new SuccessResult<T>(Ok);
            }
            return new ErrorResult<T>(Err);
        }


        public JOSResult ToDatalessJOSResult()
        {
            if (Err is null)
            {
                if (Ok != null)
                {
                    throw new Exception(
                        "Can't make a Dataless JOSResult with nonempty data. Use JOSResult<T> instead.");
                }

                // Recurse for nested results.
                // This will produce an Ok (with data) if there are Oks all the way down.
                if (Ok is ServerResult<T> serverResult)
                {
                    return serverResult.ToDatalessJOSResult();
                }

                return new SuccessResult();
            }

            return new ErrorResult(Err);
        }
    }
}
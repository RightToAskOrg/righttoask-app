using System;
using System.Text;
using RightToAskClient.Models;
using Xamarin.Essentials;

namespace RightToAskClient.Data
{
    public static class HttpUtils
    {
        // TODO Actually we only get response.OK = false here in the very specific circumstance that
        // we got a fine http response but the sig didn't verify. So this probably deserves to be
        // refactored to make that clear. In particular, it's only meaningful for communication with 
        // the RightToAsk server, and doesn't make much sense for others.
        public static (bool isValid, string message) validateHttpResponse(Result<bool> response, string messageTopic)
        {
            if (String.IsNullOrEmpty(response.Err))
            {
                if (response.Ok)
                {
                    return (true, messageTopic + ": Success.");
                }

                return (false, messageTopic + ": Failure.");
            }

            return (false, "Server connection error" + response.Err);
        }
    }
}
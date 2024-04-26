using System;

namespace ReadyPlayerMe.Runtime.Api.Common
{
    public class ApiException : Exception
    {
        public ApiError Error { get; set; }
        
        public ApiException(ApiError error)
        {
            Error = error;
        }

    }
}
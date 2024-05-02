using System;

namespace ReadyPlayerMe.Runtime.Api.Common.Exceptions
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
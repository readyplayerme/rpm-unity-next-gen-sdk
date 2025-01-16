using System;

namespace PlayerZero.Api.Exceptions
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
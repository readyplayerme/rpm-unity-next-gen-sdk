using System.Collections.Generic;

namespace ReadyPlayerMe.Runtime.Api.Common
{
    public class RequestData<T>
    {
        public string Url { get; set; }

        public string Method { get; set; }
        
        public T Payload { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
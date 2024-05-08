using System.Collections.Generic;

namespace ReadyPlayerMe.Runtime.Api
{
    public class ApiRequest<T>
    {
        public string Url { get; set; }

        public string Method { get; set; }
        
        public T Payload { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}
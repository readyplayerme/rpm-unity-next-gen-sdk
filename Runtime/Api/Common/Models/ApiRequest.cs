using System.Collections.Generic;

namespace ReadyPlayerMe.Api
{
    public class ApiRequest<T>
    {
        public string Url { get; set; }

        public string Method { get; set; }
        
        public T Payload { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}
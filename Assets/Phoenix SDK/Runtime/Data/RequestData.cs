using System.Collections.Generic;

namespace ReadyPlayerMe.Phoenix
{
    public struct RequestData
    {
        public string Url;
        public HttpMethod Method;
        public string Payload;
        public Dictionary<string, string> Headers;
    }
}
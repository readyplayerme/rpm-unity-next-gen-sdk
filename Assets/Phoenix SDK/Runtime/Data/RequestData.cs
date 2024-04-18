using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public struct WebRequestData
    {
        public string Url;
        public string Method;
        public string Payload;
        public Dictionary<string, string> Headers;
    }
}
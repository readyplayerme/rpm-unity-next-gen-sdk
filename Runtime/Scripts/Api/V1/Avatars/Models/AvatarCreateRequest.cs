using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class AvatarCreateRequest
    {
        public AvatarCreateRequestBody Payload { get; set; } = new AvatarCreateRequestBody();
    }

    public class AvatarCreateRequestBody
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }
        
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}

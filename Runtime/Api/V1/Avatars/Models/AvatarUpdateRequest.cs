using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Avatars.Models
{
    public class AvatarUpdateRequest
    {
        public string AvatarId { get; set; }

        public AvatarUpdateRequestBody Payload { get; set; } = new AvatarUpdateRequestBody();
    }
    
    public class AvatarUpdateRequestBody
    {
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}
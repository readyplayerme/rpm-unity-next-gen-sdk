using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters.Models
{
    public class CharacterCreateRequest
    {
        public CharacterCreateRequestBody Payload { get; set; } = new CharacterCreateRequestBody();
    }

    public class CharacterCreateRequestBody
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }
        
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}

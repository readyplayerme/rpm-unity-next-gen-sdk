using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters.Models
{
    public class CharacterUpdateRequest
    {
        public string CharacterId { get; set; }

        public CharacterUpdateRequestBody Payload { get; set; } = new CharacterUpdateRequestBody();
    }
    
    public class CharacterUpdateRequestBody
    {
        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; }
        
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}
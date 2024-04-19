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
        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; }
        
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}

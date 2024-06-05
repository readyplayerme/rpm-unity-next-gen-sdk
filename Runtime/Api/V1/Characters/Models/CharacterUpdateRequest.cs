using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class CharacterUpdateRequest
    {
        public string Id { get; set; }

        public CharacterUpdateRequestBody Payload { get; set; } = new CharacterUpdateRequestBody();
    }
    
    public class CharacterUpdateRequestBody
    {
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}
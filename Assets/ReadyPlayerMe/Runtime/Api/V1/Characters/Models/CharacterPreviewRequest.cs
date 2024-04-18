using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters.Models
{
    public class CharacterPreviewRequest
    {
        public string CharacterId { get; set; }

        public CharacterPreviewQueryParams Params { get; set; } = new CharacterPreviewQueryParams();
    }
    
    public class CharacterPreviewQueryParams
    {
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}
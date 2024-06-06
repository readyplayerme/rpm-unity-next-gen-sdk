using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class CharacterPreviewRequest
    {
        public string Id { get; set; }

        public CharacterPreviewQueryParams Params { get; set; } = new CharacterPreviewQueryParams();
    }
    
    public class CharacterPreviewQueryParams
    {
        [JsonProperty("assets")]
        public IDictionary<string, string> Assets { get; set; }
    }
}
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters.Models
{
    public class CharacterPreviewResponse
    {
        [JsonProperty("glbUrl")]
        public string GlbUrl { get; set; }
    }
}
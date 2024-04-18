using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Data.V1;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters.Models
{
    public class CharacterUpdateResponse
    {
        [JsonProperty("data")]
        public Character Data { get; set; }
    }
}
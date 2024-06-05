using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class CharacterCreateResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Character Data { get; set; }
    }
}
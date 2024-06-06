using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class CharacterUpdateResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Character Data { get; set; }
    }
}
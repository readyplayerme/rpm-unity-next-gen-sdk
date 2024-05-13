using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class AvatarUpdateResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Avatar Data { get; set; }
    }
}
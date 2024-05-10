using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class AvatarCreateResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Avatar Data { get; set; }
    }
}
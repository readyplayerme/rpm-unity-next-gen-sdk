using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class AvatarFindByIdResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Avatar Data { get; set; }
    }
}
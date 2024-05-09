using Newtonsoft.Json;
using ReadyPlayerMe.Data.V1;

namespace ReadyPlayerMe.Api.V1
{
    public class AvatarFindByIdResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Avatar Data { get; set; }
    }
}
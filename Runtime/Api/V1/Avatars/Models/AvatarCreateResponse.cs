using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Data.V1;

namespace ReadyPlayerMe.Runtime.Api.V1.Avatars.Models
{
    public class AvatarCreateResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Avatar Data { get; set; }
    }
}
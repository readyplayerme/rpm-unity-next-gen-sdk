using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common.Models;

namespace ReadyPlayerMe.Runtime.Api.V1.Assets.Models
{
    public class AssetTypeListResponse : ApiResponse
    {
        [JsonProperty("data")]
        public string[] Data { get; set; }
    }
}
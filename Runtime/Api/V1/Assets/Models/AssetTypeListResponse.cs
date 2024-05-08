using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1
{
    public class AssetTypeListResponse : ApiResponse
    {
        [JsonProperty("data")]
        public string[] Data { get; set; }
    }
}
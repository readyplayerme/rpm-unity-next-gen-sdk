using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class AssetListResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Asset[] Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }
}
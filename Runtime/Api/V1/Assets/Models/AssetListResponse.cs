using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Data.V1;

namespace ReadyPlayerMe.Runtime.Api.V1
{
    public class AssetListResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Asset[] Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }
}
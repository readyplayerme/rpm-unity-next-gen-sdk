using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Data.V1;

namespace ReadyPlayerMe.Runtime.Api.V1.Assets.Models
{
    public class AssetListResponse
    {
        [JsonProperty("data")]
        public Asset[] Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }
}
using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.V1.Common.Models;

namespace ReadyPlayerMe.Runtime.Api.V1.Assets.Models
{
    public class AssetListRequest
    {
        public AssetListQueryParams Params { get; set; } = new AssetListQueryParams();
    }

    public class AssetListQueryParams : PaginationQueryParams
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("excludeTypes")]
        public string ExcludeTypes { get; set; }
    }
}
using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1
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
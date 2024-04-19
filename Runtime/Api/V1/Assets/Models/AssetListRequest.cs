using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Assets.Models
{
    public class AssetListRequest
    {
        public AssetListQueryParams Params { get; set; } = new AssetListQueryParams();
    }

    public class AssetListQueryParams
    {
        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("excludeTypes")]
        public string ExcludeTypes { get; set; }
    }
}
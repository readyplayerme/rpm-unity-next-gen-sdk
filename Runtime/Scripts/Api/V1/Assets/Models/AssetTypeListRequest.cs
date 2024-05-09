using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class AssetTypeListRequest
    {
        public AssetTypeListQueryParams Params { get; set; } = new AssetTypeListQueryParams();
    }

    public class AssetTypeListQueryParams
    {
        [JsonProperty("applicationId")] 
        public string ApplicationId { get; set; }

        [JsonProperty("type")] 
        public string Type { get; set; }

        [JsonProperty("excludeTypes")] 
        public string ExcludeTypes { get; set; }
    }
}
using Newtonsoft.Json;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Api.V1
{
    public class AssetListRequest
    {
        public AssetListQueryParams Params { get; set; } = new AssetListQueryParams();
    }

    public class AssetListQueryParams : PaginationQueryParams
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; } = Resources.Load<Settings>("ReadyPlayerMeSettings")?.ApplicationId;

        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("excludeTypes")]
        public string ExcludeTypes { get; set; }
        
        [JsonProperty("characterModelAssetId")]
        public string CharacterModelAssetId { get; set; }
    }
}

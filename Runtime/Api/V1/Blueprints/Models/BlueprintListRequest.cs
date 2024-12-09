using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class BlueprintListRequest : PaginationQueryParams
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }
    }
}
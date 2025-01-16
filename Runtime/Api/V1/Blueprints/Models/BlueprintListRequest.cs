using Newtonsoft.Json;

namespace PlayerZero.Api.V1
{
    public class BlueprintListRequest : PaginationQueryParams
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }
    }
}
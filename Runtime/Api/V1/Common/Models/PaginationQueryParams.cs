using Newtonsoft.Json;

namespace ReadyPlayerMe.Api.V1
{
    public class PaginationQueryParams
    {
        [JsonProperty("limit")]
        public int Limit { get; set; } = 10;
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
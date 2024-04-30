using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.V1.Common.Models
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
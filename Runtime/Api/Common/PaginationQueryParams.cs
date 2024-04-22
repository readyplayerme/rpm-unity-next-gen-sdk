using Newtonsoft.Json;

namespace ReadyPlayerMe.Runtime.Api.Common
{
    public class PaginationQueryParams
    {
        [JsonProperty("limit")]
        public int Limit { get; set; }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
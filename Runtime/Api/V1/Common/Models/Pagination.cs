using Newtonsoft.Json;

namespace PlayerZero.Api.V1
{
    public class Pagination
    {
        [JsonProperty("totalDocs")]
        public int TotalDocs { get; set; }
        
        [JsonProperty("limit")]
        public int Limit { get; set; }
        
        [JsonProperty("TotalPages")]
        public int TotalPages { get; set; }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("pagingCounter")]
        public int PagingCounter { get; set; }
        
        [JsonProperty("hasPrevPage")]
        public bool HasPrevPage { get; set; }
        
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }
        
        [JsonProperty("prevPage")]
        public int PrevPage { get; set; }
        
        [JsonProperty("nextPage")]
        public int NextPage { get; set; }
    }
}
using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api;
using ReadyPlayerMe.Runtime.Api.V1;

namespace ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts.Models
{
    public class ApplicationListResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Application[] Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

    public class Application
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
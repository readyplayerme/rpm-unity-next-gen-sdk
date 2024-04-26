using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Common.Models;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models
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
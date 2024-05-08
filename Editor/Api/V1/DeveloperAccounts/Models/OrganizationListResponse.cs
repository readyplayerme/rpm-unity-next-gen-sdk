using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api;
using ReadyPlayerMe.Runtime.Api.V1;

namespace ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts.Models
{
    public class OrganizationListResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Organization[] Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

    public class Organization
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
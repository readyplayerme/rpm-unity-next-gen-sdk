using Newtonsoft.Json;
using ReadyPlayerMe.Runtime.Api.V1.Common.Models;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models
{
    public class ApplicationListRequest
    {
        public ApplicationListQueryParams Params { get; set; } = new ApplicationListQueryParams();
    }
    
    public class ApplicationListQueryParams : PaginationQueryParams
    {
        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; }
    }
}
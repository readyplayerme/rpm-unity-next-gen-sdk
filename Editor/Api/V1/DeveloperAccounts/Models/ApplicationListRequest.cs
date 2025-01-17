using Newtonsoft.Json;
using PlayerZero.Api.V1;

namespace PlayerZero.Editor.Api.V1.DeveloperAccounts.Models
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
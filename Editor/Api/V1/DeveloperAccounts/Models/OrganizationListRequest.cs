using PlayerZero.Api.V1;

namespace PlayerZero.Editor.Api.V1.DeveloperAccounts.Models
{
    public class OrganizationListRequest
    {
        public OrganizationListQueryParams Params { get; set; } = new OrganizationListQueryParams();
    }
    
    public class OrganizationListQueryParams : PaginationQueryParams
    {
    }
}
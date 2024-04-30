using ReadyPlayerMe.Runtime.Api.V1.Common.Models;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models
{
    public class OrganizationListRequest
    {
        public OrganizationListQueryParams Params { get; set; } = new OrganizationListQueryParams();
    }
    
    public class OrganizationListQueryParams : PaginationQueryParams
    {
    }
}
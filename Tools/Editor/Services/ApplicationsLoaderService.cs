using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models;
using ReadyPlayerMe.Tools.Editor.Data;

namespace ReadyPlayerMe.Tools.Editor.Services
{
    public class ApplicationsLoaderService
    {
        private readonly DeveloperAccountApi _developerAccountApi;

        public ApplicationsLoaderService(DeveloperAccountApi developerAccountApi)
        {
            _developerAccountApi = developerAccountApi;
        }
        
        public async Task<ServiceResponse<IList<Application>>> LoadAsync()
        {
            var organizationListResponse =
                await _developerAccountApi.ListOrganizationsAsync(new OrganizationListRequest());

            if (!organizationListResponse.IsSuccess)
                return new ServiceResponse<IList<Application>>()
                {
                    Data = new List<Application>(),
                    Error = organizationListResponse.Error,
                    IsSuccess = false,
                };
            

            var organizationId = organizationListResponse.Data[0].Id;

            var applicationListResponse = await _developerAccountApi.ListApplicationsAsync(new ApplicationListRequest
            {
                Params = new ApplicationListQueryParams
                {
                    OrganizationId = organizationId
                }
            });
            
            if (!applicationListResponse.IsSuccess)
                return new ServiceResponse<IList<Application>>()
                {
                    Data = new List<Application>(),
                    Error = applicationListResponse.Error,
                    IsSuccess = false,
                };
            
            return new ServiceResponse<IList<Application>>()
            {
                IsSuccess = true,
                Data = applicationListResponse.Data.ToList()
            };
        }
    }
}
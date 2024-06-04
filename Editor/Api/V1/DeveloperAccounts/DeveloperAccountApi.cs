using System.Threading.Tasks;
using ReadyPlayerMe.Api;
using ReadyPlayerMe.Editor.Api.V1.Auth;
using ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts
{
    public sealed class DeveloperAccountApi : WebApiWithAuth
    {
        public DeveloperAccountApi()
        {
            // LogWarnings = false;
            SetAuthenticationStrategy(new DeveloperTokenAuthStrategy());
        }

        public async Task<ApplicationListResponse> ListApplicationsAsync(ApplicationListRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            return await Dispatch<ApplicationListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/applications{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET
                }
            );
        }

        public async Task<OrganizationListResponse> ListOrganizationsAsync(OrganizationListRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            return await Dispatch<OrganizationListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/organizations{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                }
            );
        }
    }
}
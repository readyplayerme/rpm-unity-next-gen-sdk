using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts
{
    public sealed class DeveloperAccountApi : WebApiWithAuth
    {
        public DeveloperAccountApi()
        {
            SetAuthenticationStrategy(new DeveloperTokenAuthStrategy());
        }
        
        public async Task<DeveloperLoginResponse> LoginAsync(DeveloperLoginRequest request)
        {
            return await Dispatch<DeveloperLoginResponse, DeveloperLoginRequestBody>(
                new ApiRequest<DeveloperLoginRequestBody>()
                {
                    Url = $"{Settings.ApiBaseAuthUrl}/login",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    },
                    Payload = request.Payload
                }
            );
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
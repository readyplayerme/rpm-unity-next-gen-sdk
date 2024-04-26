using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Assets.Models;
using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts.Models;
using ReadyPlayerMe.Tools.Editor.Cache;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts
{
    public sealed class DeveloperAccountApi : WebApiWithDeveloperTokenRefresh
    {
        public async Task<DeveloperLoginResponse> LoginAsync(DeveloperLoginRequest request)
        {
            return await Dispatch<DeveloperLoginResponse, DeveloperLoginRequestBody>(
                new ApiRequest<DeveloperLoginRequestBody>()
                {
                    Url = "https://readyplayer.me/api/auth/login",
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
                    Url = $"{Settings.ApiBaseUrl}applications{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization",
                            $"Bearer {DeveloperDetailsCache.Data.Token}"
                        }
                    }
                }
            );
        }

        public async Task<OrganizationListResponse> ListOrganizationsAsync(OrganizationListRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            return await Dispatch<OrganizationListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}organizations{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", $"Bearer {DeveloperDetailsCache.Data.Token}" }
                    }
                }
            );
        }
        
        public async Task<AssetListResponse> ListCharacterStylesAsync(AssetListRequest request)
        {
            var queryString = BuildQueryString(request.Params);
            
            return await Dispatch<AssetListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}phoenix-assets{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Settings.Token }
                    }
                }
            );
        }
    }
}
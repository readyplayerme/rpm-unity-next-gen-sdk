using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Assets.Models;
using ReadyPlayerMe.Runtime.Api.V1.Auth.Strategies;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Assets
{
    public class AssetApi : WebApiWithAuth
    {
        private const string Resource = "phoenix-assets";

        public AssetApi()
        {
            SetAuthenticationStrategy(new ApiKeyAuthStrategy());
        }

        public virtual async Task<AssetListResponse> ListAssetsAsync(AssetListRequest request)
        {
            var queryString = BuildQueryString(request.Params);
            
            return await Dispatch<AssetListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                }
            );
        }
        
        public virtual async Task<AssetListResponse> ListAssetTypesAsync(AssetListRequest request)
        {
            var queryString = BuildQueryString(request.Params);
            
            return await Dispatch<AssetListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}/types{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                }
            );
        }
    }
}

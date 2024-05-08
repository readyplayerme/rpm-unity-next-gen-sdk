using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1
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
        
        public virtual async Task<AssetTypeListResponse> ListAssetTypesAsync(AssetTypeListRequest request)
        {
            var queryString = BuildQueryString(request.Params);
            
            return await Dispatch<AssetTypeListResponse>(new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}/types{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                }
            );
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.V1.Assets.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Assets
{
    public class AssetApi : WebApi
    {
        private const string RESOURCE = "phoenix-assets";
    
        public async virtual Task<AssetListResponse> ListAssetsAsync(AssetListRequest request)
        {
            var queryString = BuildQueryString(request.Params);
            
            return await Dispatch<AssetListResponse>(new RequestData<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}{RESOURCE}{queryString}",
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
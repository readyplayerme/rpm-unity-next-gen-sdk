using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1
{
    public class AuthApi : WebApi
    {
        public virtual async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return await Dispatch<RefreshTokenResponse, RefreshTokenRequestBody>(
                new ApiRequest<RefreshTokenRequestBody>()
                {
                    Url = $"{Settings.ApiBaseAuthUrl}/refresh",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    },
                    Payload = request.Payload
                }
            );
        }
    }
}
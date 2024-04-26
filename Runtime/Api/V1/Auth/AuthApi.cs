using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Auth.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Auth
{
    public class AuthApi : WebApi
    {
        public virtual async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return await Dispatch<RefreshTokenResponse, RefreshTokenRequestBody>(
                new ApiRequest<RefreshTokenRequestBody>()
                {
                    Url = "https://readyplayer.me/api/auth/refresh",
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
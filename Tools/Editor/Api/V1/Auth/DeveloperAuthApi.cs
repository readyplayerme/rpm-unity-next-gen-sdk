using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Tools.Editor.Api.V1.Auth.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.Auth
{
    public sealed class DeveloperAuthApi : WebApi
    {
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
    }
}
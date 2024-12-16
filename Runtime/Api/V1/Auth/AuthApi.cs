using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Api.V1
{
    public class AuthApi : WebApi
    {
        public virtual async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
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
                },
            cancellationToken);
        }

        public virtual async Task<SendLoginCodeResponse> SendLoginCodeAsync(SendLoginCodeRequest request, CancellationToken cancellationToken = default)
        {
            var payload = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );
            
            var apiRequest = new ApiRequest<string>()
            {
                Url = $"{Settings.ApiBaseUrl}/v1/auth/request-login-code",
                Method = UnityWebRequest.kHttpVerbPOST,
                Headers = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json" },
                },
                Payload = payload
            };
            return await Dispatch<SendLoginCodeResponse>(apiRequest, cancellationToken);
        }
        
        public virtual async Task<LoginWithCodeResponse> LoginWithCodeAsync(LoginWithCodeRequest request, CancellationToken cancellationToken = default)
        {
            
            var payload = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );
            
            var apiRequest = new ApiRequest<string>()
            {
                Url = $"{Settings.ApiBaseUrl}/v1/auth/login",
                Method = UnityWebRequest.kHttpVerbPOST,
                Headers = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json" },
                },
                Payload = payload
            };
            return await Dispatch<LoginWithCodeResponse>(apiRequest, cancellationToken);
        }

        public virtual async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            return await Dispatch<CreateUserResponse, CreateUserRequest>(
                new ApiRequest<CreateUserRequest>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/users",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    },
                    Payload = request
                }, cancellationToken
            );
        }
    }
}
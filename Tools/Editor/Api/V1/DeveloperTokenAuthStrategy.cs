using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common.Contracts;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Auth;
using ReadyPlayerMe.Runtime.Api.V1.Auth.Models;
using ReadyPlayerMe.Tools.Editor.Cache;

namespace ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts
{
    public class DeveloperTokenAuthStrategy : IAuthenticationStrategy
    {
        private readonly AuthApi _authApi;

        public DeveloperTokenAuthStrategy()
        {
            _authApi = new AuthApi();
        }

        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request)
        {
            request.Headers.Add("Authorization",$"Bearer {DeveloperAuthCache.Data.Token}");
            
            return Task.CompletedTask;
        }

        public async Task<bool> TryRefreshAsync<T>(ApiRequest<T> request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
                return false;

            var refreshTokenResponse = await _authApi.RefreshTokenAsync(
                new RefreshTokenRequest()
                {
                    Payload = new RefreshTokenRequestBody
                    {
                        RefreshToken = DeveloperAuthCache.Data.RefreshToken,
                        Token = DeveloperAuthCache.Data.Token,
                    }
                });

            if (!refreshTokenResponse.IsSuccess)
                DeveloperAuthCache.Delete();

            var developerDetails = DeveloperAuthCache.Data;
            developerDetails.Token = refreshTokenResponse.Data.Token;
            DeveloperAuthCache.Data = developerDetails;

            request.Headers["Authorization"] = $"Bearer {refreshTokenResponse.Data.Token}";

            return true;
        }
    }
}
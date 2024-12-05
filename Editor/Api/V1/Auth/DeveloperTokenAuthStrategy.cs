using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Api;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Editor.Cache;
using ReadyPlayerMe.Editor.Cache.EditorPrefs;

namespace ReadyPlayerMe.Editor.Api.V1.Auth
{
    public class DeveloperTokenAuthStrategy : IAuthenticationStrategy
    {
        private readonly AuthApi _authApi;

        public DeveloperTokenAuthStrategy()
        {
            _authApi = new AuthApi();
        }

        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request, CancellationToken cancellationToken = default)
        {
            request.Headers.Add("Authorization",$"Bearer {DeveloperAuthCache.Data?.Token}");

            return Task.CompletedTask;
        }

        public async Task<bool> TryRefreshAsync<T>(ApiRequest<T> request, CancellationToken cancellationToken = default)
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
                }, cancellationToken);

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
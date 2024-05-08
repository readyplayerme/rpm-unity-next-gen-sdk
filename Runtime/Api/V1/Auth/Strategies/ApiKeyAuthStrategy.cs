using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Data;
using UnityEngine;

namespace ReadyPlayerMe.Runtime.Api.V1
{
    public class ApiKeyAuthStrategy : IAuthenticationStrategy
    {
        private readonly Settings _settings;

        public ApiKeyAuthStrategy()
        {
            _settings = Resources.Load<Settings>("ReadyPlayerMeSettings");
        }
        
        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request)
        {
            request.Headers["X-API-KEY"] = _settings.ApiKey;

            return Task.CompletedTask;
        }

        public Task<bool> TryRefreshAsync<T>(ApiRequest<T> request)
        {
            return Task.FromResult(false);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Api.V1
{
    public class ApiKeyAuthStrategy : IAuthenticationStrategy
    {
        private readonly Settings _settings;

        public ApiKeyAuthStrategy()
        {
            _settings = Resources.Load<Settings>("ReadyPlayerMeSettings");
        }
        
        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request, CancellationToken cancellationToken = default)
        {
            request.Headers["X-API-KEY"] = _settings.ApiKey;

            return Task.CompletedTask;
        }

        public Task<bool> TryRefreshAsync<T>(ApiRequest<T> request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common.Contracts;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Data.ScriptableObjects;
using UnityEngine;

namespace ReadyPlayerMe.Runtime.Api.V1.Auth.Strategies
{
    public class ApiKeyAuthStrategy : IAuthenticationStrategy
    {
        private readonly Settings _settings;

        public ApiKeyAuthStrategy()
        {
            _settings = Resources.Load<Settings>("Settings");
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
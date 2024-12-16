using System.Threading;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Api
{
    public interface IAuthenticationStrategy
    {
        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request, CancellationToken cancellationToken = default);

        public Task<bool> TryRefreshAsync<T>(ApiRequest<T> request, CancellationToken cancellationToken = default);
    }
}
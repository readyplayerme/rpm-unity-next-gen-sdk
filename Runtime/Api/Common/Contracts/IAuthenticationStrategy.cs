using System.Threading.Tasks;

namespace ReadyPlayerMe.Api
{
    public interface IAuthenticationStrategy
    {
        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request);

        public Task<bool> TryRefreshAsync<T>(ApiRequest<T> request);
    }
}
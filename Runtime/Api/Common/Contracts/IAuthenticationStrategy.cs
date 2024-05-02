using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common.Models;

namespace ReadyPlayerMe.Runtime.Api.Common.Contracts
{
    public interface IAuthenticationStrategy
    {
        public Task AddAuthToRequestAsync<T>(ApiRequest<T> request);

        public Task<bool> TryRefreshAsync<T>(ApiRequest<T> request);
    }
}
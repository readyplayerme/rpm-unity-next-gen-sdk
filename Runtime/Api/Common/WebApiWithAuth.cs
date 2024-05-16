using System;
using System.Net;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Api
{
    public abstract class WebApiWithAuth : WebApi
    {
        private IAuthenticationStrategy _authenticationStrategy;

        public void SetAuthenticationStrategy(IAuthenticationStrategy authenticationStrategy)
        {
            _authenticationStrategy = authenticationStrategy;
        }

        protected override async Task<TResponse> Dispatch<TResponse, TRequestBody>(
            ApiRequest<TRequestBody> data
        )
        {
            return await WithAuth(
                async (updatedData) =>
                    await base.Dispatch<TResponse, TRequestBody>(updatedData), data);
        }

        protected override async Task<TResponse> Dispatch<TResponse>(ApiRequest<string> data)
        {
            return await WithAuth(
                async (updatedData) =>
                    await base.Dispatch<TResponse>(updatedData), data);
        }

        private async Task<TResponse> WithAuth<TResponse, TRequestBody>(
            Func<ApiRequest<TRequestBody>, Task<TResponse>> dispatchRequest,
            ApiRequest<TRequestBody> apiRequest
        ) where TResponse : ApiResponse, new()
        {
            if (_authenticationStrategy == null)
                return await dispatchRequest(apiRequest);
            
            await _authenticationStrategy.AddAuthToRequestAsync(apiRequest);

            var result = await dispatchRequest(apiRequest);

            if (result.IsSuccess)
                return result;
            
            if (result.Status != (int)HttpStatusCode.Unauthorized)
                return result;

            var refreshSucceeded = await _authenticationStrategy.TryRefreshAsync(apiRequest);
            
            if (!refreshSucceeded)
                return result;
           
            return await dispatchRequest(apiRequest);
        }
    }
}
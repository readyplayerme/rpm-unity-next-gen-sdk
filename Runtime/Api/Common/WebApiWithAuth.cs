using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PlayerZero.Api
{
    public abstract class WebApiWithAuth : WebApi
    {
        private IAuthenticationStrategy _authenticationStrategy;

        public void SetAuthenticationStrategy(IAuthenticationStrategy authenticationStrategy)
        {
            _authenticationStrategy = authenticationStrategy;
        }

        protected override async Task<TResponse> Dispatch<TResponse, TRequestBody>(
            ApiRequest<TRequestBody> data, CancellationToken cancellationToken = default)
        {
            return await WithAuth(
                async (updatedData) =>
                    await base.Dispatch<TResponse, TRequestBody>(updatedData, cancellationToken),
                data, cancellationToken);
        }

        protected override async Task<TResponse> Dispatch<TResponse>(
            ApiRequest<string> data, CancellationToken cancellationToken = default)
        {
            return await WithAuth(
                async (updatedData) =>
                    await base.Dispatch<TResponse>(updatedData, cancellationToken),
                data, cancellationToken);
        }

        private async Task<TResponse> WithAuth<TResponse, TRequestBody>(
            Func<ApiRequest<TRequestBody>, Task<TResponse>> dispatchRequest,
            ApiRequest<TRequestBody> apiRequest,
            CancellationToken cancellationToken = default)
            where TResponse : ApiResponse, new()
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_authenticationStrategy == null)
            {
                return await dispatchRequest(apiRequest);
            }

            // Add authentication to the request
            await _authenticationStrategy.AddAuthToRequestAsync(apiRequest, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var result = await dispatchRequest(apiRequest);

            // If request succeeds or is not Unauthorized, return the result
            if (result.IsSuccess || result.Status != (int)HttpStatusCode.Unauthorized)
            {
                return result;
            }

            // Try to refresh authentication
            var refreshSucceeded = await _authenticationStrategy.TryRefreshAsync(apiRequest, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            
            if (!refreshSucceeded)
            {
                return result;
            }

            // Retry the request with refreshed authentication
            return await dispatchRequest(apiRequest);
        }
    }
}

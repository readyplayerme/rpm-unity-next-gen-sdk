using System;
using System.Net;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Auth;
using ReadyPlayerMe.Runtime.Api.V1.Auth.Models;
using UnityEngine;

namespace ReadyPlayerMe.Runtime.Api.V1.Common
{
    public abstract class WebApiWithTokenRefresh : WebApi
    {
        private readonly AuthApi _authApi;
        
        protected WebApiWithTokenRefresh()
        {
            _authApi = new AuthApi();
        }

        protected override async Task<TResponse> Dispatch<TResponse, TRequestBody>(ApiRequest<TRequestBody> data)
        {
            return await WithTokenRefresh(async () => await base.Dispatch<TResponse, TRequestBody>(data), data);
        }

        protected override async Task<TResponse> Dispatch<TResponse>(ApiRequest<string> data)
        {
            return await WithTokenRefresh(async () => await base.Dispatch<TResponse>(data), data);
        }

        private async Task<TResponse> WithTokenRefresh<TResponse, TRequestBody>(Func<Task<TResponse>> dispatchRequest,
            ApiRequest<TRequestBody> apiRequest) where TResponse : ApiResponse, new()
        {
            var result = await dispatchRequest();

            if (result.IsSuccess)
                return result;
            
            if (!apiRequest.Headers.ContainsKey("Authorization"))
                return result;
            
            Debug.Log(result.Status);

            if (result.Status != (int)HttpStatusCode.Unauthorized)
                return result;

            var refreshTokenResponse = await _authApi.RefreshTokenAsync(
                new RefreshTokenRequest()
                {
                    Payload = new RefreshTokenRequestBody
                    {
                        RefreshToken = GetRefreshToken(),
                        Token = GetAccessToken(),
                    }
                });
            
            Debug.Log(refreshTokenResponse.Data.RefreshToken);
            Debug.Log(refreshTokenResponse.IsSuccess);

            if (!refreshTokenResponse.IsSuccess)
            {
                DeleteAccessToken();
                return result;
            }

            SetAccessToken(refreshTokenResponse.Data.Token);

            return await dispatchRequest();
        }

        protected abstract string GetAccessToken();

        protected abstract string GetRefreshToken();

        protected abstract void SetAccessToken(string token);

        protected abstract void DeleteAccessToken();
    }
}
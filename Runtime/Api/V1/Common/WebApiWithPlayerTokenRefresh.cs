using System;

// TODO: change this to support API Keys and Player Tokens
namespace ReadyPlayerMe.Runtime.Api.V1.Common
{
    public class WebApiWithPlayerTokenRefresh : WebApiWithTokenRefresh
    {
        protected override string GetAccessToken()
        {
            return Settings.Token;
        }

        protected override string GetRefreshToken()
        {
            throw new NotImplementedException();
        }

        protected override void SetAccessToken(string token)
        {
            Settings.Token = token;
        }

        protected override void DeleteAccessToken()
        {
            Settings.Token = "";
        }
    }
}
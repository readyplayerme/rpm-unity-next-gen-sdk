using ReadyPlayerMe.Runtime.Api.V1.Common;
using ReadyPlayerMe.Tools.Editor.Cache;

namespace ReadyPlayerMe.Tools.Editor.Api.V1
{
    public class WebApiWithDeveloperTokenRefresh : WebApiWithTokenRefresh
    {
        protected override string GetAccessToken()
        {
            return DeveloperDetailsCache.Data.Token;
        }

        protected override string GetRefreshToken()
        {
            return DeveloperDetailsCache.Data.RefreshToken;
        }

        protected override void SetAccessToken(string token)
        {
            var developerDetails = DeveloperDetailsCache.Data;
            developerDetails.Token = token;
            DeveloperDetailsCache.Data = developerDetails;
        }

        protected override void DeleteAccessToken()
        {
            DeveloperDetailsCache.Delete();
        }
    }
}
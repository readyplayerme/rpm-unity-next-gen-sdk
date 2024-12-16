using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Api.V1
{
    public class CharacterApi : WebApiWithAuth
    {
        private const string Resource = "characters";

        public CharacterApi()
        {
            SetAuthenticationStrategy(new ApiKeyAuthStrategy());
        }

        public virtual async Task<CharacterFindByIdResponse> FindByIdAsync(CharacterFindByIdRequest request, CancellationToken cancellationToken = default)
        {
            return await Dispatch<CharacterFindByIdResponse>(
                new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}/{request.Id}",
                    Method = "GET",
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    }
                }, cancellationToken
            );
        }

        public virtual string GeneratePreviewUrl(CharacterPreviewRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            return $"{Settings.ApiBaseUrl}/v1/{Resource}/{request.Id}/preview{queryString}";
        }
    }
}
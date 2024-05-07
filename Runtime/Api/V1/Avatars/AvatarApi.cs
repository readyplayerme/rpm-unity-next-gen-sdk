using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Auth.Strategies;
using ReadyPlayerMe.Runtime.Api.V1.Avatars.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Avatars
{
    public class AvatarApi : WebApiWithAuth
    {
        private const string Resource = "next-gen-avatars";

        public AvatarApi()
        {
            SetAuthenticationStrategy(new ApiKeyAuthStrategy());
        }

        public virtual async Task<AvatarCreateResponse> CreateAvatarAsync(AvatarCreateRequest request)
        {
            return await Dispatch<AvatarCreateResponse, AvatarCreateRequestBody>(
                new ApiRequest<AvatarCreateRequestBody>
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    }
                }
            );
        }

        public virtual async Task<AvatarUpdateResponse> UpdateAvatarAsync(AvatarUpdateRequest request)
        {
            return await Dispatch<AvatarUpdateResponse, AvatarUpdateRequestBody>(
                new ApiRequest<AvatarUpdateRequestBody>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}/{request.AvatarId}",
                    Method = "PATCH",
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    }
                }
            );
        }

        public virtual async Task<AvatarFindByIdResponse> FindAvatarByIdAsync(AvatarFindByIdRequest request)
        {
            return await Dispatch<AvatarFindByIdResponse>(
                new ApiRequest<string>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}/{request.AvatarId}",
                    Method = UnityWebRequest.kHttpVerbGET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    }
                }
            );
        }

        public virtual string GenerateAvatarPreviewUrl(AvatarPreviewRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            return $"{Settings.ApiBaseUrl}/v1/{Resource}/{request.AvatarId}/preview{queryString}";
        }
    }
}
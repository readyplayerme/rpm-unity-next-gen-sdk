using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.Common.Models;
using ReadyPlayerMe.Runtime.Api.V1.Auth.Strategies;
using ReadyPlayerMe.Runtime.Api.V1.Characters.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters
{
    public class CharacterApi : WebApiWithAuth
    {
        private const string Resource = "characters";

        public CharacterApi()
        {
            SetAuthenticationStrategy(new ApiKeyAuthStrategy());
        }
        
        public virtual async Task<CharacterCreateResponse> CreateCharacterAsync(CharacterCreateRequest request)
        {
            return await Dispatch<CharacterCreateResponse, CharacterCreateRequestBody>(
                new ApiRequest<CharacterCreateRequestBody>
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

        public virtual async Task<CharacterUpdateResponse> UpdateCharacterAsync(CharacterUpdateRequest request)
        {
            return await Dispatch<CharacterUpdateResponse, CharacterUpdateRequestBody>(
                new ApiRequest<CharacterUpdateRequestBody>()
                {
                    Url = $"{Settings.ApiBaseUrl}/v1/{Resource}/{request.CharacterId}",
                    Method = "PATCH",
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                    }
                }
            );
        }
        
        public virtual async Task<GltfImport> PreviewCharacterAsync(CharacterPreviewRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            var gltf = new GltfImport();
            
            await gltf.Load($"{Settings.ApiBaseUrl}/v1/{Resource}/{request.CharacterId}/preview{queryString}");

            return gltf;
        }
    }
}
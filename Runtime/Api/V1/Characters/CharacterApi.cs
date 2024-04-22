using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Runtime.Api.Common;
using ReadyPlayerMe.Runtime.Api.V1.Characters.Models;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Characters
{
    public class CharacterApi : WebApi
    {
        private const string RESOURCE = "characters";

        public virtual async Task<CharacterCreateResponse> CreateCharacterAsync(CharacterCreateRequest request)
        {
            return await Dispatch<CharacterCreateResponse, CharacterCreateRequestBody>(
                new RequestData<CharacterCreateRequestBody>
                {
                    Url = $"{Settings.ApiBaseUrl}{RESOURCE}",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                        { "Authorization", Settings.Token }
                    }
                }
            );
        }

        public virtual async Task<CharacterUpdateResponse> UpdateCharacterAsync(CharacterUpdateRequest request)
        {
            return await Dispatch<CharacterUpdateResponse, CharacterUpdateRequestBody>(
                new RequestData<CharacterUpdateRequestBody>()
                {
                    Url = $"{Settings.ApiBaseUrl}{RESOURCE}/{request.CharacterId}",
                    Method = "PATCH",
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                        { "Authorization", Settings.Token }
                    }
                }
            );
        }

        // TODO: Change this to return a game object
        public virtual async Task<GltfImport> PreviewCharacterAsync(CharacterPreviewRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            var gltf = new GltfImport();
            
            await gltf.Load($"{Settings.ApiBaseUrl}{RESOURCE}/{request.CharacterId}/preview{queryString}");

            return gltf;
        }
    }
}
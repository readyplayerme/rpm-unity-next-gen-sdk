using System.Collections.Generic;
using System.Threading.Tasks;
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
                    Url = $"{Constants.API_V1_BASE_URL}{RESOURCE}",
                    Method = UnityWebRequest.kHttpVerbPOST,
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                        { "Authorization", Constants.TOKEN }
                    }
                }
            );
        }

        public virtual async Task<CharacterUpdateResponse> UpdateCharacterAsync(CharacterUpdateRequest request)
        {
            return await Dispatch<CharacterUpdateResponse, CharacterUpdateRequestBody>(
                new RequestData<CharacterUpdateRequestBody>()
                {
                    Url = $"{Constants.API_V1_BASE_URL}{RESOURCE}/{request.CharacterId}",
                    Method = "PATCH",
                    Payload = request.Payload,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" },
                        { "Authorization", Constants.TOKEN }
                    }
                }
            );
        }

        public virtual async Task<CharacterPreviewResponse> PreviewCharacterAsync(CharacterPreviewRequest request)
        {
            var queryString = BuildQueryString(request.Params);

            return await Dispatch<CharacterPreviewResponse>(new RequestData<string>
                {
                    Url = $"{Constants.API_V1_BASE_URL}{RESOURCE}/{request.CharacterId}{queryString}",
                    Method = UnityWebRequest.kHttpVerbGET,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Authorization", Constants.TOKEN }
                    }
                }
            );
        }
    }
}
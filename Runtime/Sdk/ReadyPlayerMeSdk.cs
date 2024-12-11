using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using UnityEngine;

namespace ReadyPlayerMe.Sdk
{
    public static class ReadyPlayerMeSdk
    {
        private static CharacterApi _characterApi;

        private static void Init()
        {
            if (_characterApi == null)
                _characterApi = new CharacterApi();
        }
        
        public static async Task<GameObject> LoadAvatarAsync(string avatarId)
        {
            Init();
            
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = avatarId,
            });

            var characterMetadata = response.Data;

            // TODO: do template loading here

            var gltf = new GltfImport();

            if (!await gltf.Load(characterMetadata.ModelUrl))
                return null;

            var characterObject = new GameObject(characterMetadata.Id);

            await gltf.InstantiateSceneAsync(characterObject.transform);
            
            // TODO: do mesh transfer here

            return characterObject;
        }
    }
}
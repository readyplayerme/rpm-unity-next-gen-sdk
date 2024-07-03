using UnityEngine;
using ReadyPlayerMe.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class CharacterManager
    {
        public static readonly Dictionary<string, CharacterData> Characters = new Dictionary<string, CharacterData>();

        private readonly CharacterLoader _characterLoader = new CharacterLoader();
        private readonly MeshTransfer _meshTransfer = new MeshTransfer();

        public async Task<CharacterData> LoadCharacter(string id, string templateTagOrId = null)
        {
            Characters.TryGetValue(id, out var characterData);

            if (characterData != null)
            {
                if (templateTagOrId != null)
                {
                    return await UpdateNew(id, characterData, templateTagOrId);
                }
                
                return await Update(id, characterData);
            }

            return await Create(id, templateTagOrId);
        }

        private async Task<CharacterData> Create(string id, string templateTagOrId)
        {
            var data = await _characterLoader.LoadAsync(id, templateTagOrId);

            Characters.Add(id, data);

            return data;
        }

        private async Task<CharacterData> Update(string id, CharacterData original)
        {
            var data = await _characterLoader.LoadAsync(id);

            _meshTransfer.Transfer(data.gameObject, original.gameObject);
            Object.Destroy(data.gameObject);
            
            return original;
        }
        
        private async Task<CharacterData> UpdateNew(string id, CharacterData original, string templateTagOrId)
        {
            var data = await _characterLoader.LoadAsyncX(id, templateTagOrId, original.gameObject);

            return data;
        }
    }
}
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
            if (templateTagOrId != null || !Characters.TryGetValue(id, out var characterData))
            {
                return await Create(id, templateTagOrId);
            }

            return await Update(id, characterData);
        }

        private async Task<CharacterData> Create(string id, string templateTagOrId)
        {
            var data = await _characterLoader.LoadAsync(id);
            
            Characters.TryGetValue(id, out var characterData);
            if(characterData != null)
            {
                Object.Destroy(characterData.gameObject);
            }
            
            Characters[id] = data;

            return data;
        }

        private async Task<CharacterData> Update(string id, CharacterData original)
        {
            var data = await _characterLoader.LoadAsync(id);

            _meshTransfer.Transfer(data.gameObject, original.gameObject);
            Object.Destroy(data.gameObject);
            
            return original;
        }
    }
}
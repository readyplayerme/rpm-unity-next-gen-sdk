using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.CharacterLoader
{
    public class CharacterManager
    {
        public static readonly Dictionary<string, CharacterData> Characters = new Dictionary<string, CharacterData>();

        private readonly CharacterLoader _characterLoader = new CharacterLoader();
        private readonly MeshTransfer _meshTransfer = new MeshTransfer();

        public async Task<CharacterData> LoadCharacter(string glbUrl, string id)
        {
            Characters.TryGetValue(id, out var characterData);

            if (characterData != null)
            {
                return await Update(glbUrl, id, characterData);
            }

            return await Create(glbUrl, id);
        }

        private async Task<CharacterData> Create(string glbUrl, string id)
        {
            var data = await _characterLoader.LoadAsync(id, loadFrom: glbUrl);

            Characters.Add(id, data);

            return data;
        }

        private async Task<CharacterData> Update(string glbUrl, string id, CharacterData original)
        {
            var data = await _characterLoader.LoadAsync(id, loadFrom: glbUrl);

            _meshTransfer.Transfer(data.gameObject, original.gameObject);
            Object.Destroy(data.gameObject);

            return original;
        }
    }
}
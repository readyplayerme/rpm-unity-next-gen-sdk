using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class InplaceAvatarLoader : MonoBehaviour
    {
        public string avatarId;

        private AvatarLoader _avatarLoader;

        public void Awake()
        {
            _avatarLoader = new AvatarLoader();
        }

        public async void Start()
        {
            if (!string.IsNullOrEmpty(avatarId))
            {
                await LoadAsync(avatarId);
            }
        }

        public async Task<GameObject> PreviewAsync(Dictionary<string, string> assets)
        {
            var avatar = await _avatarLoader.PreviewAsync(avatarId, assets, gameObject);
            
            avatarId = avatar.GetComponent<AvatarData>().Id;

            return avatar;
        }

        public async Task<GameObject> LoadAsync(string id)
        {
            var avatar = await _avatarLoader.LoadAsync(id, gameObject);
            
            avatarId = avatar.GetComponent<AvatarData>().Id;

            return avatar;
        }
    }
}
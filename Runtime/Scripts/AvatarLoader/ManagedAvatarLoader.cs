using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class ManagedAvatarLoader
    {
        private const string TEMPLATE_PATH = "Template/template_prefab";
        
        public static Dictionary<string, AvatarData> loadedAvatars = new Dictionary<string, AvatarData>();
        
        private MeshTransfer meshTransfer = new MeshTransfer();
        private SkeletonBuilder skeletonBuilder = new SkeletonBuilder();
        
        public async Task<GameObject> LoadAvatar(string glbUrl, string id)
        {
            loadedAvatars.TryGetValue(id, out AvatarData avatarData);
            
            if(avatarData != null)
            {
                return await UpdateAvatar(glbUrl, avatarData.gameObject);
            }
            
            return await CreateAvatar(glbUrl, id);
        }
        
        private async Task<GameObject> CreateAvatar(string glbUrl, string id)
        {
            var gltf = new GltfImport();
            if (await gltf.Load(glbUrl))
            {
                // Load template
                GameObject template = Resources.Load<GameObject>(TEMPLATE_PATH);
                GameObject instance = Object.Instantiate(template);
                
                // Load avatar
                GameObject avatar = new GameObject("avatar");
                await gltf.InstantiateSceneAsync(avatar.transform);
                
                // Update skeleton and transfer mesh
                skeletonBuilder.Build(instance);
                meshTransfer.Transfer(avatar, instance);
                
                // Set avatar data
                AvatarData data = instance.AddComponent<AvatarData>();
                data.Initialize(id);
                instance.name = data.Id;
                loadedAvatars.Add(data.Id, data);
                
                return instance;
            }
            
            return null;
        }
        
        private async Task<GameObject> UpdateAvatar(string glbUrl, GameObject original)
        {
            var gltf = new GltfImport();
            if (await gltf.Load(glbUrl))
            {
                GameObject avatar = new GameObject("temp_avatar");
                await gltf.InstantiateSceneAsync(avatar.transform);
                meshTransfer.Transfer(avatar, original);
                
                Object.Destroy(avatar);
            }

            return original;
        }
    }
}

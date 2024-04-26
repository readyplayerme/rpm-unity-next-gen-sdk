using GLTFast;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReadyPlayerMe.Runtime.Data;
using ReadyPlayerMe.Runtime.Utils;

namespace ReadyPlayerMe.Runtime.Loader
{
    public class AvatarLoader
    {
        public static Dictionary<string, AvatarData> loadedAvatars = new Dictionary<string, AvatarData>();
        
        public async Task<GameObject> LoadAvatar(string glbUrl)
        {
            string id = UrlUtils.GetIdFromUrl(glbUrl);
            loadedAvatars.TryGetValue(id, out AvatarData avatarData);
            
            if(avatarData != null)
            {
                return await UpdateAvatar(glbUrl, avatarData.gameObject);
            }
            
            return await CreateAvatar(glbUrl);
        }
        
        private async Task<GameObject> CreateAvatar(string glbUrl)
        {
            var gltf = new GltfImport();
            if (await gltf.Load(glbUrl))
            {
                // Load template
                GameObject template = Resources.Load<GameObject>("Template/template_prefab");
                GameObject instance = UnityEngine.Object.Instantiate(template);
                
                // Load avatar
                GameObject avatar = new GameObject("avatar");
                await gltf.InstantiateSceneAsync(avatar.transform);
                
                // Update skeleton and transfer mesh
                SkeletonUtils.ApplySkeleton(instance);
                MeshUtils.TransferMesh(avatar, instance);
                
                // Set avatar data
                AvatarData data = instance.AddComponent<AvatarData>();
                data.Initialize(glbUrl);
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
                MeshUtils.TransferMesh(avatar, original);
                
                UnityEngine.Object.Destroy(avatar);
            }

            return original;
        }
    }
}

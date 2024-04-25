using GLTFast;
using UnityEngine;
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Utils;

namespace ReadyPlayerMe.Runtime.Loader
{
    public class AvatarLoader
    {
        public async Task<GameObject> LoadAvatar(string glbUrl)
        {
            var gltf = new GltfImport();
            if (await gltf.Load(glbUrl))
            {
                GameObject template = Resources.Load<GameObject>("Template/template_prefab");
                GameObject instance = Object.Instantiate(template);
                
                GameObject avatar = new GameObject("avatar");
                await gltf.InstantiateSceneAsync(avatar.transform);
                SkeletonUtils.ApplySkeleton(instance);
                MeshUtils.TransferMesh(avatar, instance);
                return instance;
            }
            
            return null;
        }
        
        public async Task LoadAvatar(string glbUrl, GameObject original)
        {
            var gltf = new GltfImport();
            if (await gltf.Load(glbUrl))
            {
                GameObject avatar = new GameObject("avatar");
                await gltf.InstantiateSceneAsync(avatar.transform);
                MeshUtils.TransferMesh(avatar, original);
            }
        }
    }
}

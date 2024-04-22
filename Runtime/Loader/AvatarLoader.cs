using GLTFast;
using UnityEngine;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Runtime.Loader
{
    public class AvatarLoader
    {
        public async Task<GameObject> LoadAvatar(string glbUrl)
        {
            var gltf = new GltfImport();
            if (await gltf.Load(glbUrl))
            {
                GameObject avatar = new GameObject("avatar");
                await gltf.InstantiateSceneAsync(avatar.transform);
                
                return avatar;
            }
            
            return null;
        }
    }
}

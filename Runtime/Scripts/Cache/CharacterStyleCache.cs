using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Cache
{
    public class CharacterStyleCache : CacheBase
    {
        public CharacterStyleCache() : base("Character Templates") {}

        public async Task Save(byte[] bytes, string id)
        {
            var path = $"{CacheDirectory}/{id}.glb";
            await File.WriteAllBytesAsync(path, bytes);
                
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            AssetDatabase.Refresh();
        }
        
        public GameObject Load(string id)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(
                $"{CacheDirectory}/{id}.glb");

            return asset;
        }
    }
}
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.Cache
{
    public class GlbCache : Cache
    {
        public GlbCache(string name) : base(name) {}

        public async Task Save(byte[] bytes, string id)
        {
            var path = $"{CacheDirectory}/{id}.glb";
            await File.WriteAllBytesAsync(path, bytes);
                
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public GameObject Load(string id)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>($"{CacheDirectory}/{id}.glb");
        }
    }
}
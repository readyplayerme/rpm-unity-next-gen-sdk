using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.Cache
{
    public class ObjectCache<T> : Cache where T : Object
    {
        public ObjectCache(string name) : base(name) { }

        public void Delete(string id)
        {
            AssetDatabase.DeleteAsset($"{CacheDirectory}/{id}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public void Import(T cache, string id)
        {
            AssetDatabase.DeleteAsset($"{CacheDirectory}/{id}.asset");
            
            if (cache == null)
                return;

            var guid = FindAssetGuid(cache);
            var path = AssetDatabase.GUIDToAssetPath(guid);

            AssetDatabase.CopyAsset(path, $"{CacheDirectory}/{id}.asset");
            EditorUtility.SetDirty(cache);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
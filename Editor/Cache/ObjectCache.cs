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

        public string GetCacheId(string id)
        {
            return AssetDatabase.AssetPathToGUID($"{CacheDirectory}/{id}.asset");
        }
        
        public void Save(T cache, string id)
        {
            AssetDatabase.DeleteAsset($"{CacheDirectory}/{id}.asset");
            
            if (cache == null)
                return;

            AssetDatabase.CreateAsset(cache, $"{CacheDirectory}/{id}.asset");
            EditorUtility.SetDirty(cache);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public void ImportToResources(T cacheId, string id)
        {
            var guid = FindAssetGuid(cacheId);
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.MoveAsset(assetPath, $"{CacheDirectory}/{id}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public void ExportFromResources(string id)
        {
            AssetDatabase.MoveAsset($"{CacheDirectory}/{id}.asset", $"Assets/{id}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.Cache
{
    public class ScriptableObjectCacheWriter<T> : CacheWriterBase where T : ScriptableObject
    {
        public ScriptableObjectCacheWriter(string name) : base(name) { }
        
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
    }
}
using System.IO;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Cache
{
    public class CharacterDataCache<T> : CacheBase where T : Object
    {
        public CharacterDataCache(string name) : base(name) { }
        
        public void Save(T template, string id)
        {
            AssetDatabase.DeleteAsset($"{CacheDirectory}/{id}.txt");
            
            if (template == null)
                return;

            var assetGuid = FindAssetGuid(template);

            File.WriteAllText($"{CacheDirectory}/{id}.txt", assetGuid.ToString());
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public GameObject Load(string characterStyleId)
        {
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(
                $"{CacheDirectory}/{characterStyleId}.txt");

            if (text == null)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(text.text);

            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        public string GetCacheId(string id)
        {
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(
                $"{CacheDirectory}/{id}.txt");;

            if (text == null)
                return null;

            return text.text;
        }
        
        private static GUID FindAssetGuid(T asset)
        {
            var guids = new NativeArray<GUID>(new GUID[1]
                {
                    GUID.Generate(),
                },
                Allocator.Temp
            );

            AssetDatabase.InstanceIDsToGUIDs(
                new NativeArray<int>(new int[] { asset.GetInstanceID() }, Allocator.Temp),
                guids
            );

            return guids[0];
        }
    }
}
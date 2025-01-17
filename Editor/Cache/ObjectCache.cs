﻿using UnityEditor;
using UnityEngine;

namespace PlayerZero.Editor.Cache
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
    }
}
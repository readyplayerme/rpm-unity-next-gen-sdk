using System;
using UnityEngine;
using UnityEditor;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CacheCreatorViewModel
    {
        private readonly string cacheFolderPath = Application.persistentDataPath + "/Local Cache/Assets"; 
        
        public event Action OnCacheGenerated;
        public event Action OnRemoteCacheDownloaded;
        
        public async void GenerateCache(int cacheItemCount)
        {
            var cache = new CacheGenerator();
            await cache.GenerateCache(cacheItemCount);
            OnCacheGenerated?.Invoke();
        }
        
        public void ExtractCache()
        {
            var cache = new CacheGenerator();
            cache.ExtractCache();
            EditorUtility.RevealInFinder(cacheFolderPath);
        }
        
        public void OpenCacheFolder()
        {
            EditorUtility.RevealInFinder(cacheFolderPath);
        }
        
        public async void DownloadAndExtractRemoveCache(string url)
        {
            var cache = new CacheGenerator();
            await cache.DownloadAndExtract(url);
            EditorUtility.RevealInFinder(cacheFolderPath);
            OnRemoteCacheDownloaded?.Invoke();
        }
    }
}

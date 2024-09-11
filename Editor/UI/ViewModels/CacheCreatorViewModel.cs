using System;
using UnityEditor;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CacheCreatorViewModel
    {
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
            EditorUtility.RevealInFinder(CachePaths.CACHE_ASSET_ROOT);
        }
        
        public void OpenCacheFolder()
        {
            EditorUtility.RevealInFinder(CachePaths.CACHE_ASSET_ROOT);
        }
        
        public async void DownloadAndExtractRemoveCache(string url)
        {
            var cache = new CacheGenerator();
            await cache.DownloadAndExtract(url);
            EditorUtility.RevealInFinder(CachePaths.CACHE_ASSET_ROOT);
            OnRemoteCacheDownloaded?.Invoke();
        }
    }
}

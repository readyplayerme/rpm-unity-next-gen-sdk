using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Api.V1
{
    public class FileApi
    {
        
        public virtual async Task<Texture2D> DownloadImageAsync(string url)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            var op = request.SendWebRequest();
        
            while (!op.isDone)
            {
                await Task.Yield();
            }
        
            return DownloadHandlerTexture.GetContent(request);
        }
        
        // TODO: Move the string path to a class of constants
        public virtual async Task<Texture2D> DownloadAssetIconAsync(Asset asset)
        {
            if(!File.Exists(CachePaths.CACHE_ASSET_ICON_PATH + asset.Id))
            {
                // download thumbnail
                using UnityWebRequest iconRequest = UnityWebRequest.Get(asset.IconUrl);
                iconRequest.downloadHandler = new DownloadHandlerFile(CachePaths.CACHE_ASSET_ICON_PATH + asset.Id);
                AsyncOperation iconOp = iconRequest.SendWebRequest();
                while (!iconOp.isDone) await Task.Yield();
            }
            
            return await GetTextureFromFile(CachePaths.CACHE_ASSET_ICON_PATH + asset.Id);
        }
        
        private async Task<Texture2D> GetTextureFromFile(string path)
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            return texture;
        }

        public virtual async Task<Texture2D[]> DownloadImagesAsync(IEnumerable<string> urls)
        {
            return await Task.WhenAll(urls.Select(DownloadImageAsync));
        }
        
        public virtual async Task<byte[]> DownloadFileIntoMemoryAsync(string url)
        {
            using var request = new UnityWebRequest();
            request.url = url;
            request.downloadHandler = new DownloadHandlerBuffer();

            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download file: " + request.error);
            }
            else
            {
                return request.downloadHandler.data;
            }

            return null;
        }
    }
}
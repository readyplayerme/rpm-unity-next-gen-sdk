using System.Collections.Generic;
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
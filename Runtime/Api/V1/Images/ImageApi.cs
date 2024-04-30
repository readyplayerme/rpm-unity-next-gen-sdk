using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Runtime.Api.V1.Images
{
    public class ImageApi
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
    }
}
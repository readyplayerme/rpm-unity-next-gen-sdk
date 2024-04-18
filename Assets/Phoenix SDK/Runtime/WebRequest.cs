using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    public static class WebRequest
    {
        public async static Task<T> Dispatch<T>(WebRequestData data)
        {
            using var request = new UnityWebRequest();
            request.url = data.Url;
            request.method = data.Method;
            request.downloadHandler = new DownloadHandlerBuffer();

            if (data.Headers != null)
            {
                foreach (var header in data.Headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(data.Payload))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data.Payload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
        
            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }
        
            T response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        
            return response;
        }
    }
}
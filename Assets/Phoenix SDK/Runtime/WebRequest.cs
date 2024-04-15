using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe.Phoenix
{
    public static class WebRequest
    {
        public async static Task<T> Dispatch<T>(RequestData data)
        {
            using var request = new UnityWebRequest();
            request.url = data.Url;
            request.method = MethodToString(data.Method);
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
        
        private static string MethodToString(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return UnityWebRequest.kHttpVerbGET;
                case HttpMethod.POST:
                    return UnityWebRequest.kHttpVerbPOST;
                case HttpMethod.PUT:
                    return UnityWebRequest.kHttpVerbPUT;
                case HttpMethod.DELETE:
                    return UnityWebRequest.kHttpVerbDELETE;
                default:
                    return UnityWebRequest.kHttpVerbGET;
            }
        }
    }
}
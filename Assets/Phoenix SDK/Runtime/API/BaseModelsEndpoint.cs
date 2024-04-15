using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;
using ReadyPlayerMe.Phoenix.Data;

namespace ReadyPlayerMe.Phoenix
{
    public class BaseModelsEndpoint
    {
        private const string ENDPOINT = "v1/organization-base-models";
        
        public async Task<BaseModelsResponse> Get()
        {
            using var request = new UnityWebRequest();
            request.url = $"{Constants.BASE_URL}/{ENDPOINT}";
            request.method = UnityWebRequest.kHttpVerbGET;
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Authorization", Constants.TOKEN);

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            BaseModelsResponse response = JsonConvert.DeserializeObject<BaseModelsResponse>(request.downloadHandler.text);

            return response;
        }
    }
}

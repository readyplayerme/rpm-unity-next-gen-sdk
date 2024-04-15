using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Threading.Tasks;
using ReadyPlayerMe.Phoenix.Data;

public class CharactersEndpoint
{
    private const string ENDPOINT = "v1/characters";
    
    public async Task<CharactersResponse> CreateAvatar(CharactersRequestData data)
    {
        using var request = new UnityWebRequest();
        request.url = $"{Constants.BASE_URL}/{ENDPOINT}";
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.downloadHandler = new DownloadHandlerBuffer();
        
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", Constants.TOKEN);
        
        string body = JsonConvert.SerializeObject(new CharactersRequest()
        {
            data = data
        });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        
        UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }
        
        CharactersResponse response = JsonConvert.DeserializeObject<CharactersResponse>(request.downloadHandler.text);
        
        return response;
    }
    
    // /v1/characters/661577f8ff0fc89c6f92ca55/preview?assets=661446cbcc77dbcde05a55b1
    public async Task<CharactersResponse> PreviewAssetOnCharacter(string characterId, string assetId)
    {
        using var request = new UnityWebRequest();
        request.url = $"{Constants.BASE_URL}/{ENDPOINT}/{characterId}/preview?assets={assetId}";
        request.method = UnityWebRequest.kHttpVerbGET;
        request.downloadHandler = new DownloadHandlerBuffer();
        
        request.SetRequestHeader("Authorization", Constants.TOKEN);
        
        UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }
        
        CharactersResponse response = JsonConvert.DeserializeObject<CharactersResponse>(request.downloadHandler.text);
        
        return response;
    }
}

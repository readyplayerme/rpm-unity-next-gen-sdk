using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class BuildLocalApi
{
    [MenuItem("Tools/Local Api Folder")]
    public static void OpenLocalApiFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath + "/Local Cache/Assets/");
    }
    
    [MenuItem("Tools/Build Local Api")]
    public async static void Build()
    {
        // make request
        AssetApi assetApi = new AssetApi();
        
        AssetListResponse baseModelResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params = new AssetListQueryParams()
            {
                Type = "baseModel",
                Limit = Int32.MaxValue
            }
        });

        foreach (var baseModel in baseModelResponse.Data)
        {
            Debug.Log(baseModel.Id);
        }
        
        AssetListResponse assetListResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params = new AssetListQueryParams()
            {
                ExcludeTypes = "baseModel",
                Limit = Int32.MaxValue
            }
        });
        
        // assuming asset is reftted to basemodel
        foreach (var baseModel in baseModelResponse.Data)
        {
            string folderPath = Application.persistentDataPath + "/Local Cache/Assets/" + baseModel.Id;
            
            if (!Directory.Exists(folderPath + baseModel.Id))
            {
                Directory.CreateDirectory(folderPath + baseModel.Id);
            }
            
            foreach (var asset in assetListResponse.Data)
            {
                using(UnityWebRequest request = UnityWebRequest.Get(asset.GlbUrl))
                {
                    request.downloadHandler = new DownloadHandlerFile(folderPath + baseModel.Id + "/" + asset.Id);
                    AsyncOperation op = request.SendWebRequest();
                    
                    while (!op.isDone) await Task.Yield();
                }
            }
        }
    }
}

using System.IO;
using UnityEditor;
using UnityEngine;
using ReadyPlayerMe;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;

public class BuildLocalApi
{
    [MenuItem("Tools/Local API/Show Cache Folder")]
    public static void OpenLocalApiFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath + "/Local Cache/Assets/");
    }
    
    // Developer will be able to call such method to build the local api
    [MenuItem("Tools/Local API/Build Local Api")]
    public async static void Build()
    {
        AssetApi assetApi = new AssetApi();
        
        // get all base models
        AssetListResponse baseModelResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params = new AssetListQueryParams()
            {
                Type = "baseModel",
                Limit = 10
            }
        });
        
        // assuming asset is reftted to basemodels, iterate over base models and request their assets
        foreach (var baseModel in baseModelResponse.Data)
        {
            // get all assets except base models of that base model
            // not available at api level yet so this is a mock
            AssetListResponse assetListResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
            {
                Params = new AssetListQueryParams()
                {
                    ExcludeTypes = "baseModel",
                    Limit = 10,
                    CharacterModelAssetId = baseModel.Id
                }
            });
            
            string folderPath = Application.streamingAssetsPath + "/Local Cache/Assets/" + baseModel.Id;
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // download assets
            foreach (Asset asset in assetListResponse.Data)
            {
                // download glb
                using UnityWebRequest glbRequest = UnityWebRequest.Get(asset.GlbUrl);
                glbRequest.downloadHandler = new DownloadHandlerFile(folderPath + "/" + asset.Id);
                AsyncOperation glbOp = glbRequest.SendWebRequest();
                while (!glbOp.isDone) await Task.Yield();
                asset.GlbUrl = asset.Id;
                
                // download thumbnail
                using UnityWebRequest iconRequest = UnityWebRequest.Get(asset.IconUrl);
                iconRequest.downloadHandler = new DownloadHandlerFile(folderPath + "/" + asset.Id + ".png");
                AsyncOperation iconOp = iconRequest.SendWebRequest();
                while (!iconOp.isDone) await Task.Yield();
                asset.IconUrl = asset.Id + ".png";
            }
            
            // save json for requests
            string jsonPath = folderPath + "/assets.json";
            await File.WriteAllTextAsync(jsonPath, JsonConvert.SerializeObject(assetListResponse.Data, Formatting.Indented));
        }
        
        // create zip file
        string zipPath = Application.streamingAssetsPath + "/Local Cache/Assets.zip";
        string assetsPath = Application.streamingAssetsPath + "/Local Cache/Assets"; 
        File.Delete(zipPath);
        ZipFile.CreateFromDirectory(assetsPath, zipPath );
        
        // delete assets folder
        Directory.Delete(assetsPath, true);
    }
    
    // Developer will be able to call such method to unpack the local api into the cache folder
    [MenuItem("Tools/Local API/Unpack Local Api")]
    public static void Unpack()
    {
        string zipPath = Application.streamingAssetsPath + "/Local Cache/Assets.zip";
        string assetsPath = Application.persistentDataPath + "/Local Cache/Assets"; 
        
        if (File.Exists(zipPath))
        {
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }
            
            ZipFile.ExtractToDirectory(zipPath, assetsPath, true);
        }
        
        EditorUtility.RevealInFinder(assetsPath);
    }

    [MenuItem("Tools/Local API/Load Avatar From Cache")]
    public async static void LoadAnAvatarFromCache()
    {
        var baseModelIds = new [] {"6683ce21d69622d88d9c70cd", "665e05e758e847063761c985"};
        KeyValuePair<string, string> assetItem = KeyValuePair.Create("top", "665e06ab455affc599ea98c0");
        CharacterApi characterApi = new CharacterApi();
        CharacterManager characterManager = new CharacterManager();

        foreach (var baseModelId in baseModelIds)
        {
            var createResponse = await characterApi.CreateAsync(new CharacterCreateRequest()
            {
                Payload = new CharacterCreateRequestBody()
                {
                    Assets = new Dictionary<string, string>()
                    {
                        { assetItem.Key, assetItem.Value },
                        { "baseModel", baseModelId }
                    }
                }
            });
            
            if (CehckIfExistsInCache(baseModelId, assetItem.Value))
            {
                await characterManager.LoadCharacter(createResponse.Data.Id, baseModelId);
            }
        }
    }
    
    // This will be implemented in character loader
    private static bool CehckIfExistsInCache(string basemodelId, string assetId)
    {
        string path = Application.persistentDataPath + "/Local Cache/Assets/" + basemodelId + "/" + assetId;
        return File.Exists(path);
    }
    
    [MenuItem("Tools/Local API/Mock API Request from JSON")]
    private async static void MockApiRequest()
    {
        // some payload
        var request = new
        {
            baseModelId = "665e05e758e847063761c985",
            type = "top",
            limit = 10
        };
    }
    
    // Mutate the json after caching an asset that does not exist in the cache
    private static void AddItemToLocalCache(string characterModelId, Asset asset)
    {
        // load json
        string jsonPath = Application.streamingAssetsPath + "/Local Cache/Assets/" + characterModelId + "/assets.json";
        string json = File.ReadAllText(jsonPath);
        List<Asset> assets = JsonConvert.DeserializeObject<List<Asset>>(json);
        
        // check if id exists
        if (!assets.Exists(a => a.Id == asset.Id))
        {
            // add item
            assets.Add(asset);
        }
        
        // save json
        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(assets, Formatting.Indented));
    }
}

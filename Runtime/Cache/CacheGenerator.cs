using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CacheGenerator
{
    private readonly string cacheZipFolderPath = Application.persistentDataPath + "/Local Cache/Assets.zip";
    private readonly string folderPath = Application.streamingAssetsPath + "/Local Cache/Assets/";
    private readonly string zipFilePath = Application.streamingAssetsPath + "/Local Cache/Assets.zip";
    private readonly string cacheFolderPath = Application.persistentDataPath + "/Local Cache/Assets";
    
    public async Task DownloadAndExtract(string url)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerFile(cacheZipFolderPath);
        AsyncOperation op = request.SendWebRequest();
        while (!op.isDone)
        {
            await Task.Yield();
        }
            
        if (File.Exists(zipFilePath))
        {
            if (!Directory.Exists(cacheFolderPath))
            {
                Directory.CreateDirectory(cacheFolderPath);
            }
            
            ZipFile.ExtractToDirectory(cacheZipFolderPath, cacheFolderPath, true);
        }
        
        File.Delete(cacheZipFolderPath);
    }

    public async Task GenerateCache(int cacheItemCount)
    {
        AssetApi assetApi = new AssetApi();
        List<Asset> assetList = new List<Asset>();
    
        // get all base models
        AssetListResponse baseModelResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params = new AssetListQueryParams()
            {
                Type = "baseModel",
                Limit = int.MaxValue
            }
        });
    
        // get all asset types
        AssetTypeListResponse assetTypeListResponse = await assetApi.ListAssetTypesAsync(new AssetTypeListRequest());
        string[] assetTypes = assetTypeListResponse.Data;
        
        // assuming asset is reftted to basemodels, iterate over base models and request their assets
        bool assetListGenerated = false;
        foreach (var baseModel in baseModelResponse.Data)
        {
            if (!Directory.Exists(folderPath+ baseModel.Id))
            {
                Directory.CreateDirectory(folderPath + baseModel.Id);
            }

            foreach (string assetType in assetTypes)
            {
                // get all assets except base models
                AssetListResponse assetListResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
                {
                    Params = new AssetListQueryParams()
                    {
                        Limit = cacheItemCount,
                        Type = assetType
                    }
                });
                
                if(!assetListGenerated)
                {
                    assetList.AddRange(assetListResponse.Data);
                }
                
                // download assets
                foreach (Asset asset in assetListResponse.Data)
                {
                    // download glb
                    using UnityWebRequest glbRequest = UnityWebRequest.Get(asset.GlbUrl);
                    glbRequest.downloadHandler = new DownloadHandlerFile(folderPath + baseModel.Id + "/" + asset.Id);
                    AsyncOperation glbOp = glbRequest.SendWebRequest();
                    while (!glbOp.isDone) await Task.Yield();
                }
                
                string iconsFolderPath = folderPath + "Icons";
                if (!Directory.Exists(iconsFolderPath))
                {
                    Directory.CreateDirectory(iconsFolderPath);
                }
                // download assets
                foreach (Asset asset in assetListResponse.Data)
                {
                    // download thumbnail
                    using UnityWebRequest iconRequest = UnityWebRequest.Get(asset.IconUrl);
                    iconRequest.downloadHandler = new DownloadHandlerFile(iconsFolderPath + "/" + asset.Id);
                    AsyncOperation iconOp = iconRequest.SendWebRequest();
                    while (!iconOp.isDone) await Task.Yield();
                }
            }
            assetListGenerated = true;
        }
        
        // save json for requests
        string assetsJsonPath = folderPath + "/assets.json";
        await File.WriteAllTextAsync(assetsJsonPath, JsonConvert.SerializeObject(assetList, Formatting.Indented));
        
        string assetTypesJsonPath = folderPath + "/types.json";
        await File.WriteAllTextAsync(assetTypesJsonPath, JsonConvert.SerializeObject(assetTypes, Formatting.Indented));
        
        // create zip file
        File.Delete(zipFilePath);
        ZipFile.CreateFromDirectory(folderPath, zipFilePath);
        
        // delete assets folder
        Directory.Delete(folderPath, true);
    }

    public void ExtractCache()
    {
        if (File.Exists(zipFilePath))
        {
            if (Directory.Exists(cacheFolderPath))
            {
                Directory.Delete(cacheFolderPath, true);
            }
            ZipFile.ExtractToDirectory(zipFilePath, cacheFolderPath, true);
        }
    }
}

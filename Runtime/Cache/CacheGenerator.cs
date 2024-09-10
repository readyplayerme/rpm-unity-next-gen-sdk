using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class CacheGenerator
    {
        public async Task DownloadAndExtract(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerFile(CachePaths.CACHE_ASSET_ZIP_PATH);
            AsyncOperation op = request.SendWebRequest();
            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (File.Exists(CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH))
            {
                if (!Directory.Exists(CachePaths.CACHE_ASSET_ROOT))
                {
                    Directory.CreateDirectory(CachePaths.CACHE_ASSET_ROOT);
                }

                ZipFile.ExtractToDirectory(CachePaths.CACHE_ASSET_ZIP_PATH, CachePaths.CACHE_ASSET_ROOT, true);
            }

            File.Delete(CachePaths.CACHE_ASSET_ZIP_PATH);
        }

        public async Task GenerateCache(int cacheItemCount)
        {
            AssetApi assetApi = new AssetApi();
            List<CachedAsset> assetList = new List<CachedAsset>();

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
            
            foreach (var baseModel in baseModelResponse.Data)
            {
                if (!Directory.Exists(CachePaths.PROJECT_CACHE_ASSET_ROOT + baseModel.Id))
                {
                    Directory.CreateDirectory(CachePaths.PROJECT_CACHE_ASSET_ROOT + baseModel.Id);
                }

                foreach (string assetType in assetTypes)
                {
                    // get all assets except base models
                    AssetListResponse assetListResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
                    {
                        Params = new AssetListQueryParams()
                        {
                            Limit = cacheItemCount,
                            Type = assetType,
                            CharacterModelAssetId = baseModel.Id
                        }
                    });
                    
                    foreach (Asset asset in assetListResponse.Data)
                    {
                        CachedAsset cachedAsset = assetList.FirstOrDefault(ca => ca.Id == asset.Id);
                        if (cachedAsset == null)
                        {
                            cachedAsset = asset;
                            cachedAsset.GlbUrls = new Dictionary<string, string>();
                            assetList.Add(cachedAsset);
                        }
                        cachedAsset.GlbUrls.Add(baseModel.Id, asset.GlbUrl);
                    }

                    // TODO: Ignore downloading base model glb files ??
                    foreach (Asset asset in assetListResponse.Data)
                    {
                        // download glb
                        using UnityWebRequest glbRequest = UnityWebRequest.Get(asset.GlbUrl);
                        glbRequest.downloadHandler = new DownloadHandlerFile(CachePaths.PROJECT_CACHE_ASSET_ROOT + baseModel.Id + "/" + asset.Id);
                        AsyncOperation glbOp = glbRequest.SendWebRequest();
                        while (!glbOp.isDone) await Task.Yield();
                    }
                    
                    if (!Directory.Exists(CachePaths.PROJECT_CACHE_ASSET_ICON_PATH))
                    {
                        Directory.CreateDirectory(CachePaths.PROJECT_CACHE_ASSET_ICON_PATH);
                    }
                    // download assets
                    foreach (Asset asset in assetListResponse.Data)
                    {
                        // download thumbnail
                        using UnityWebRequest iconRequest = UnityWebRequest.Get(asset.IconUrl);
                        iconRequest.downloadHandler = new DownloadHandlerFile(CachePaths.PROJECT_CACHE_ASSET_ICON_PATH + "/" + asset.Id);
                        AsyncOperation iconOp = iconRequest.SendWebRequest();
                        while (!iconOp.isDone) await Task.Yield();
                    }
                }
            }

            // save json for requests
            await File.WriteAllTextAsync(CachePaths.PROJECT_CACHE_ASSET_JSON_PATH, JsonConvert.SerializeObject(assetList, Formatting.Indented));

            await File.WriteAllTextAsync(CachePaths.PROJECT_CACHE_TYPES_JSON_PATH, JsonConvert.SerializeObject(assetTypes, Formatting.Indented));

            if (File.Exists(CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH))
                File.Delete(CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH);
            
            await Task.Yield();
            
            ZipFile.CreateFromDirectory(CachePaths.PROJECT_CACHE_ASSET_ROOT, CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH);

            // delete assets folder
            Directory.Delete(CachePaths.PROJECT_CACHE_ASSET_ROOT, true);
        }

        public void ExtractCache()
        {
            if (File.Exists(CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH))
            {
                if (Directory.Exists(CachePaths.CACHE_ASSET_ROOT))
                {
                    Directory.Delete(CachePaths.CACHE_ASSET_ROOT, true);
                }
                ZipFile.ExtractToDirectory(CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH, CachePaths.CACHE_ASSET_ROOT, true);
            }
        }
    }
}

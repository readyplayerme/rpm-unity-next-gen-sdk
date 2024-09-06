using GLTFast;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    /// <summary>
    /// The AssetLoader class is responsible for managing the loading and listing of assets.
    /// It interacts with an external API through the AssetApi class and optionally handles
    /// cached data stored locally on the device.
    /// </summary>
    public class AssetLoader
    {
        private readonly MeshTransfer meshTransfer;
        
        private readonly AssetApi assetApi;
        private readonly string cacheFilePath = Application.persistentDataPath + "/Local Cache/Assets/assets.json";
        private readonly string cacheRoot = Application.persistentDataPath + "/Local Cache/Assets/";
        
        public readonly Dictionary<string, Asset> Assets = new Dictionary<string, Asset>();
        
        /// <summary>
        /// Initializes a new instance of the AssetLoader class.
        /// Sets up the connection to the AssetApi for retrieving assets.
        /// </summary>
        public AssetLoader()
        {
            assetApi = new AssetApi();
            meshTransfer = new MeshTransfer();
        }
        
        /// <summary>
        /// Asynchronously retrieves a list of assets based on the given request parameters.
        /// Optionally, it can return cached data if requested.
        /// </summary>
        /// <param name="request">The request object containing parameters for asset retrieval.</param>
        /// <param name="useCache">A boolean indicating whether to use cached data if available.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an AssetListResponse object,
        /// which includes the list of assets and pagination details.
        /// </returns>
        public async Task<AssetListResponse> ListAssetsAsync(AssetListRequest request, bool useCache = false)
        {
            if(useCache)
            {
                if (request.Params.CharacterModelAssetId == null && request.Params.Type != "baseModel")
                {
                    throw new System.ArgumentException("Character model asset ID is required for cached asset retrieval.");
                }
                
                int limit = request.Params.Limit;
                int page = request.Params.Page;
                string type = request.Params.Type;
                
                Asset[] allAssets = await LoadAssetsFromCacheAsync(request.Params.CharacterModelAssetId);
                Asset[] assetsOfType = allAssets.Where(asset => asset.Type == type).ToArray();
                Asset[] assetsOfPage = assetsOfType.Skip(limit * (page - 1)).Take(limit).ToArray();
                
                return new AssetListResponse()
                {
                    Data = assetsOfPage,
                    Pagination = new Pagination
                    {
                        Limit = limit,
                        Page = page,
                        TotalPages = Mathf.CeilToInt(assetsOfType.Length / (float)limit),
                        HasNextPage = page < Mathf.CeilToInt(assetsOfType.Length / (float)limit),
                        HasPrevPage = page > 1,
                        NextPage = page < Mathf.CeilToInt(assetsOfType.Length / (float)limit) ? page + 1 : page,
                        PrevPage = page > 1 ? page - 1 : page
                    }
                };
            }
            
            return await assetApi.ListAssetsAsync(request);
        }

        /// <summary>
        /// Asynchronously retrieves a list of asset types based on the given request parameters.
        /// Optionally, it can return cached data if requested.
        /// </summary>
        /// <param name="request">The request object containing parameters for asset type retrieval.</param>
        /// <param name="useCache">A boolean indicating whether to use cached data if available.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an AssetTypeListResponse object,
        /// which includes the list of asset types.
        /// </returns>
        public async Task<AssetTypeListResponse> ListAssetTypesAsync(AssetTypeListRequest request, bool useCache = false)
        {
            if(useCache)
            {
                string folderPath = Application.persistentDataPath + "/Local Cache/Assets/types.json";
                string json = await File.ReadAllTextAsync(folderPath);
                string[] types = JsonConvert.DeserializeObject<string[]>(json);
                types = types.Except(new []{ request.Params.ExcludeTypes }).ToArray();
                        
                return new AssetTypeListResponse()
                {
                    Data = types,
                };
            }
            
            return await assetApi.ListAssetTypesAsync(request);
        }

        /// <summary>
        ///     Asynchronously retrieves an asset model based on the given asset and template tag or ID.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="templateTagOrId"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public async Task<GameObject> GetAssetModelAsync(Asset asset, string templateTagOrId, bool useCache = false)
        {
            var gltf = new GltfImport();
            var outfit = new GameObject(asset.Id);

            string path = $"{cacheRoot}/{templateTagOrId}/{asset.Id}";
            byte[] assetBytes = useCache ? await DownloadOrLoadFromCache(asset.GlbUrl, path) : await File.ReadAllBytesAsync(path);

            await gltf.Load(assetBytes);
            await gltf.InstantiateSceneAsync(outfit.transform);

            Assets[asset.Type] = asset;

            return outfit;
        }
        
        /// <summary>
        ///     Asynchronously loads assets from a local cache file.
        /// </summary>
        private async Task<Asset[]> LoadAssetsFromCacheAsync(string characterModelAssetId)
        {
            if (!File.Exists(cacheFilePath))
                throw new FileNotFoundException("Cache file not found.", cacheFilePath);
        
            string json = await File.ReadAllTextAsync(cacheFilePath);
            CachedAsset[] cachedAssets = JsonConvert.DeserializeObject<CachedAsset[]>(json);
            Asset[] assets = cachedAssets.Select(cachedAsset =>
            {
                Asset asset = cachedAsset;
                if(characterModelAssetId != null) 
                    asset.GlbUrl = cachedAsset.GlbUrls[characterModelAssetId];
                
                return asset;
            }).ToArray();
            
            return assets;
        }
        
        /// <summary>
        ///     Asynchronously downloads an asset from a given URL or loads it from a local cache file.
        /// </summary>
        private async Task<byte[]> DownloadOrLoadFromCache(string url, string filePath)
        {
            if (File.Exists(filePath))
            {
                return await File.ReadAllBytesAsync(filePath);
            }

            using var request = UnityWebRequest.Get(url);
            AsyncOperation op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                byte[] data = request.downloadHandler.data;
                await File.WriteAllBytesAsync(filePath, data);
                return data;
            }

            Debug.LogError(request.error);
            return null;
        }
        
        /// <summary>
        ///     Swaps an asset on a character with a new asset.
        /// </summary>
        /// <param name="original">The character data object to swap the asset on.</param>
        /// <param name="asset">The new asset to swap in.</param>
        /// <param name="outfit">The outfit game object to swap in.</param>
        public void SwapAsset(CharacterData original, Asset asset, GameObject outfit)
        {
            if(original.AssetMeshes.ContainsKey(asset.Type))
            {
                foreach (var mesh in original.AssetMeshes[asset.Type])
                {
                    if(mesh != null)
                        Object.Destroy(mesh.gameObject);
                }
            
                original.AssetMeshes.Remove(asset.Type);
            }

            var meshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
            original.AssetMeshes.Add(asset.Type, meshes);
        
            meshTransfer.TransferMeshes(original.transform, outfit.transform, original.transform);
            Object.Destroy(outfit);
        }
    }
}

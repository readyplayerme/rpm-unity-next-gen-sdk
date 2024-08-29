using GLTFast;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    /// <summary>
    /// The AssetLoader class is responsible for managing the loading and listing of assets.
    /// It interacts with an external API through the AssetApi class and optionally handles
    /// cached data stored locally on the device.
    /// </summary>
    public class AssetLoader
    {
        private readonly AssetApi assetApi;
        private readonly string cacheFilePath = Application.persistentDataPath + "/Local Cache/Assets/assets.json";
        private readonly string cacheRoot = Application.persistentDataPath + "/Local Cache/Assets/";
        
        /// <summary>
        /// Initializes a new instance of the AssetLoader class.
        /// Sets up the connection to the AssetApi for retrieving assets.
        /// </summary>
        public AssetLoader()
        {
            assetApi = new AssetApi();
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
                if (request.Params.CharacterModelAssetId == null)
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
                    Pagination = BuildPagination(request.Params.Page, request.Params.Limit, assetsOfType.Length)
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
                types = types.Where(t => t != request.Params.ExcludeTypes).ToArray();
                        
                return new AssetTypeListResponse()
                {
                    Data = types,
                };
            }
            
            return await assetApi.ListAssetTypesAsync(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="templateTagOrId"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public async Task<GameObject> GetAssetModelAsync(Asset asset, string templateTagOrId, bool useCache = false)
        {
            byte[] assetBytes = null;
            var gltf = new GltfImport();
            var outfit = new GameObject(asset.Id);
            
            if (useCache)
            {
                assetBytes = await GetAssetBytesFromCache(asset, templateTagOrId);
            }

            // TODO: internet check Application.internetReachability != NetworkReachability.NotReachable
            if (assetBytes == null)
            {
                string path = cacheRoot + templateTagOrId + "/" + asset.Id;
                using UnityWebRequest request = UnityWebRequest.Get(asset.GlbUrl);
                request.downloadHandler = new DownloadHandlerFile(path);
                AsyncOperation glbOp = request.SendWebRequest();
                while (!glbOp.isDone) await Task.Yield();
                assetBytes = await File.ReadAllBytesAsync(path);
            }

            await gltf.Load(assetBytes);
            await gltf.InstantiateSceneAsync(outfit.transform);
            return outfit;
        }
        
        /// <summary>
        /// Asynchronously loads assets from a local cache file.
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
                asset.GlbUrl = cachedAsset.GlbUrls[characterModelAssetId];
                return asset;
            }).ToArray();
            
            return assets;
        }

        /// <summary>
        /// Builds pagination details based on the current page, limit per page, and total number of assets.
        /// </summary>
        /// <param name="page">The current page number.</param>
        /// <param name="limit">The maximum number of assets per page.</param>
        /// <param name="assetLength">The total number of assets available.</param>
        /// <returns>A Pagination object containing pagination details.</returns>
        private Pagination BuildPagination(int page, int limit, int assetLength)
        {
            Pagination pagination = new Pagination();
            pagination.Limit = limit;
            pagination.Page = page;
            pagination.TotalPages = Mathf.CeilToInt(assetLength / (float)limit);
            pagination.HasNextPage = pagination.TotalPages > page;
            pagination.HasPrevPage = page > 1;
            pagination.NextPage = pagination.HasNextPage ? page + 1 : page;
            pagination.PrevPage = pagination.HasPrevPage ? page - 1 : page;
            
            return pagination;
        }
        
        private async Task<byte[]> GetAssetBytesFromCache(Asset asset, string templateId)
        {
            string folderPath = $"{Application.persistentDataPath}/Local Cache/Assets/{templateId}";
            string filePath = $"{folderPath}/{asset.Id}";
            byte[] assetBytes;

            // Check if the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        
            // Check if the file exists in the cache and return the bytes if it does
            if(File.Exists(filePath))
            {
                assetBytes = await File.ReadAllBytesAsync(filePath);
            }
            // If not, download the asset and save it to the cache and return the bytes
            else
            {
                using UnityWebRequest request = UnityWebRequest.Get(asset.GlbUrl);
                AsyncOperation op = request.SendWebRequest();
            
                while (!op.isDone)
                {
                    await Task.Yield();
                }
            
                if (request.result == UnityWebRequest.Result.Success)
                {
                    assetBytes = request.downloadHandler.data;
                    await File.WriteAllBytesAsync(filePath, assetBytes);
                }
                else
                {
                    Debug.LogError(request.error);
                    return null;
                }
            }
        
            return assetBytes;
        }
    }
}

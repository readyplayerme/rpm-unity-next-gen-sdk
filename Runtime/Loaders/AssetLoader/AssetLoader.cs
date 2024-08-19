using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;

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
                int limit = request.Params.Limit;
                int page = request.Params.Page;
                string type = request.Params.Type;
                
                Asset[] allAssets = await LoadAssetsFromCacheAsync();
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
        /// Asynchronously loads assets from a local cache file.
        /// </summary>
        private async Task<Asset[]> LoadAssetsFromCacheAsync()
        {
            if (!File.Exists(cacheFilePath))
                throw new FileNotFoundException("Cache file not found.", cacheFilePath);
        
            string json = await File.ReadAllTextAsync(cacheFilePath);
            return JsonConvert.DeserializeObject<Asset[]>(json);
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
    }
}
using UnityEngine;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Samples.UseCache
{
    public class AssetPageController : MonoBehaviour
    {
        [SerializeField] private int assetPerPage = 12;
        [SerializeField] private AssetButton assetButtonPrefab;
        [SerializeField] private Transform assetButtonContainer;
        [SerializeField] private Paginator paginator;

        private AssetLoader assetLoader;

        /// <summary>
        ///     Load assets from the API and create buttons for each asset.
        /// </summary>
        /// <param name="category">Selected category.</param>
        /// <param name="useCache">Use cache for loading assets.</param>
        /// <param name="page">Current page.</param>
        public async void LoadAssets(string category, string characterModelAssetId, bool useCache, int page = 1)
        {
            assetLoader = new AssetLoader();

            AssetListResponse response = await assetLoader.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams()
                {
                    Type = category,
                    Limit = assetPerPage,
                    Page = page,
                    CharacterModelAssetId = characterModelAssetId
                }
            }, useCache);
            LoadPage(response.Data);
            paginator.Initialize(response.Pagination);

            EventAggregator.Instance.RaiseAssetsLoaded();
        }

        // TODO: Buttons can be cached and reinitialized with new asset information
        // Clean the page and then create buttons for each asset
        private void LoadPage(Asset[] assets)
        {
            foreach (Transform child in assetButtonContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Asset asset in assets)
            {
                AssetButton button = Instantiate(assetButtonPrefab, assetButtonContainer);
                button.Initialize(asset);
            }
        }
    }
}
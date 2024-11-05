using UnityEngine;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;

namespace ReadyPlayerMe.Demo
{
    public class AssetPageController : MonoBehaviour
    {
        [SerializeField] private int assetPerPage = 12;
        [SerializeField] private AssetButton assetButtonPrefab;
        [SerializeField] private Transform assetButtonContainer;
        [SerializeField] private Paginator paginator;
        
        private AssetApi assetApi;
        private AudioSource audioSource;
        private List<AssetButton> assetButtons = new List<AssetButton>();

        public Dictionary<string, string> EquipedAssets { get; } = new Dictionary<string, string>()
        {
            { "baseModel", "665e05e758e847063761c985" }
        };
        
        private Asset selectedAsset;
        private string cachedAssetId;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            EventAggregator.Instance.OnAssetSelected += OnAssetSelected;
            EventAggregator.Instance.OnAssetEquipped += OnAssetEquipped;
            EventAggregator.Instance.OnAssetUnequipped += OnAssetUnequipped;
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
        }

        private void OnAssetSelected(Asset asset)
        {
            foreach (AssetButton button in assetButtons)
            {
                bool isSelected = button.Asset.Id == asset.Id;
                button.SetSelected(isSelected);
            }
            
            // cache current asset incase use does not equip the selected asset
            if (EquipedAssets.TryGetValue(asset.Type, out string currentAssetId))
            {
                cachedAssetId = currentAssetId;  
            }
            
            selectedAsset = asset;
            EquipedAssets[asset.Type] = asset.Id;
        }
        
        private void OnAssetEquipped(Asset asset)
        {
            selectedAsset = null;
            EquipedAssets[asset.Type] = asset.Id;
        }
        
        private void OnAssetUnequipped(Asset asset)
        {
            selectedAsset = null;
            EquipedAssets.Remove(asset.Type);
        }
        
        private void OnCategorySelected(string category)
        {
            if (selectedAsset != null)
            {
                EquipedAssets.Remove(selectedAsset.Type);
                
                if (!string.IsNullOrEmpty(cachedAssetId) && selectedAsset.Id != cachedAssetId)
                {
                    EquipedAssets[selectedAsset.Type] = cachedAssetId;
                }
            }
        }

        /// <summary>
        ///     Load assets from the API and create buttons for each asset.
        /// </summary>
        /// <param name="category">Selected category.</param>
        /// <param name="page">Current page.</param>
        public async void LoadAssets(string category, int page = 1)
        {
            assetApi = new AssetApi();

            AssetListResponse response = await assetApi.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams()
                {
                    Type = category,
                    Limit = assetPerPage,
                    Page = page
                }
            });
            LoadPage(response.Data);
            SetupPaginator(response.Pagination);
            
            if (category == "baseModel" && !EquipedAssets.ContainsKey("baseModel"))
            {
                EquipedAssets["baseModel"] = response.Data[0].Id;
            }
            
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
            
            assetButtons.Clear();

            foreach (Asset asset in assets)
            {
                AssetButton button = Instantiate(assetButtonPrefab, assetButtonContainer);
                button.Initialize(asset, audioSource);
                assetButtons.Add(button);
                button.SetSelected(EquipedAssets.ContainsKey(asset.Type) && EquipedAssets[asset.Type] == asset.Id);
            }
        }
        
        private void SetupPaginator(Pagination pagination)
        { 
            bool displayPaginator = pagination.TotalPages > 1;
            paginator.gameObject.SetActive(displayPaginator);
            if (displayPaginator)
            {
                paginator.Initialize(pagination);
            }
        }
    }
}

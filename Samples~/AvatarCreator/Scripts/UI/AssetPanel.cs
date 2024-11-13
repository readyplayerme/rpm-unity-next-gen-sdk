using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Api.V1;
using UnityEngine;


namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class AssetPanel : MonoBehaviour
    {
        public Action<Asset> OnAssetSelected;
        public Action<Asset> OnAssetRemoved;
        [SerializeField] private int assetPerPage = 20;
        [SerializeField] private AssetButton assetButtonPrefab;
        [SerializeField] private Transform assetButtonContainer;

        private AssetApi assetApi;
        private AssetButton selectedAssetButton;
        private List<AssetButton> assetButtons = new List<AssetButton>();
        private int currentPage = 1;

        private void OnDestroy()
        {
            foreach (var assetButton in assetButtons)
            {
                Destroy(assetButton.gameObject);
            }
            assetButtons.Clear();
        }
        
        public void CreateAssetButton(Asset asset)
        {
            var button = Instantiate(assetButtonPrefab, assetButtonContainer);
            button.Initialize(asset);
            button.OnAssetClicked += OnAssetClicked;
            assetButtons.Add(button);
        }

        private void OnAssetClicked(AssetButton assetButton)
        {
            if (selectedAssetButton != null && !selectedAssetButton.Asset.Id.Contains(assetButton.Asset.Id))
            {
                selectedAssetButton.SetSelected(false);
            }
            selectedAssetButton = assetButton;
            if (assetButton.IsSelected)
            {
                OnAssetSelected?.Invoke(assetButton.Asset);
            }
            else
            {
                OnAssetRemoved?.Invoke(assetButton.Asset);
            }
        }

        public async void LoadAssetsOfCategory(string category, int page = 1)
        {
            if(assetApi == null)
            {
                assetApi = new AssetApi();
            }
            var response = await assetApi.ListAssetsAsync(new AssetListRequest()
            {
                Params = new AssetListQueryParams()
                 {
                     Type = category,
                     Page = page,
                     Limit = assetPerPage
                 }
            });
            var assets = response.Data;
            foreach (var asset in assets)
            {
                CreateAssetButton(asset);
            }
            SetDefaultSelectedAsset();
        }
        
        public void SetDefaultSelectedAsset()
        {
            if (selectedAssetButton != null)
            {
                selectedAssetButton.SetSelected(false);
            }
            if (assetButtons.Count> 1 && assetButtons[0].Asset.IsStyleAsset())
            {
                selectedAssetButton = assetButtons[0];
                selectedAssetButton.SetSelected(true);
                Debug.Log( "SET DEFAULT STYLE" );
                return;
            }
            var defaultAssets = assetButtons.Where( asset => asset.Asset.Name.EndsWith("_Default")).ToArray();
            if(defaultAssets.Length == 0)
            {
                Debug.Log( "No default asset found" );
                return;
            }
            selectedAssetButton = defaultAssets[0];
            selectedAssetButton.SetSelected(true);
            Debug.Log( $"Default asset selected {defaultAssets[0].Asset.Name}" );
        }
    }
}
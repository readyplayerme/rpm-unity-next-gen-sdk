using System;
using System.Collections.Generic;
using ReadyPlayerMe.Api.V1;
using UnityEngine;


namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class AssetPanel : MonoBehaviour
    {
        public Action<Asset> OnAssetSelected;
        [SerializeField] private int assetPerPage = 12;
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
            if (selectedAssetButton != null)
            {
                selectedAssetButton.SetSelected(false);
            }
            selectedAssetButton = assetButton;
            selectedAssetButton.SetSelected(true);
            OnAssetSelected?.Invoke(assetButton.Asset);
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
                     Page = page
                 }
            });
            var assets = response.Data;
            foreach (var asset in assets)
            {
                CreateAssetButton(asset);
            }
        }

    }
}
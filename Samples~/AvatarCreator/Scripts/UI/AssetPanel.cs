using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReadyPlayerMe.Api.V1;
using Unity.VisualScripting;
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
        [SerializeField] private float updateInterval = 5f; 

        private AssetApi assetApi;
        private AssetButton selectedAssetButton;
        private List<AssetButton> assetButtons = new List<AssetButton>();
        private int currentPage = 1;
        
        private string assetCategory;
        private Coroutine updateCoroutine;
        
        private CancellationTokenSource cancellationTokenSource;
        
        private void OnEnable()
        {
            updateCoroutine = StartCoroutine(CheckForAssetUpdatesPeriodically());
        }

        private void OnDisable()
        {
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }
        }
        
        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
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
            assetCategory = category;

            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var response = await assetApi.ListAssetsAsync(new AssetListRequest()
                {
                    Params = new AssetListQueryParams()
                    {
                        Type = category,
                        Page = page,
                        Limit = assetPerPage
                    }
                }, cancellationTokenSource.Token);
                if( cancellationTokenSource.Token.IsCancellationRequested ) return;
                
                var assets = response.Data;
                
                CreateButtons(assets);
                SetDefaultSelectedAsset();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Asset loading was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load assets: {ex.Message}");
            }
            finally
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }

        private void CreateButtons(Asset[] assets)
        {
            foreach (var asset in assets)
            {
                CreateAssetButton(asset);
            }
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
        }

        public async void CheckForAssetUpdates()
        {
            if(assetApi == null)
            {
                assetApi = new AssetApi();
            }
            var response = await assetApi.ListAssetsAsync(new AssetListRequest()
            {
                Params = new AssetListQueryParams()
                {
                    Type = assetCategory,
                    Page = 1,
                    Limit = assetPerPage
                }
            });
            var assets = response.Data;
            if (assets.Length == assetButtons.Count) return;
            var previouslySelectedAsset = selectedAssetButton ? selectedAssetButton.Asset.Id : "";
            foreach (var assetButton in assetButtons)
            {
                Destroy(assetButton.gameObject);
            }
            assetButtons.Clear();
            CreateButtons(assets);
            var newSelectedAsset = assetButtons.FirstOrDefault(asset => asset.Asset.Id == previouslySelectedAsset);
            if (newSelectedAsset == null) return;
            selectedAssetButton = newSelectedAsset;
            selectedAssetButton.SetSelected(true);
        }
        
        private IEnumerator CheckForAssetUpdatesPeriodically()
        {
            while (true)
            {
                yield return new WaitForSeconds(updateInterval);
                CheckForAssetUpdates();
            }
        }
    }
}
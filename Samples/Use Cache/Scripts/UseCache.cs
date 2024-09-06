using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Samples.UseCache
{
    public class UseCache : MonoBehaviour
    {
        [SerializeField] private CategoryController categoryController;
        [SerializeField] private AssetPageController assetPageController;
        [Space]
        [SerializeField] private Toggle categoriesToggle;
        [SerializeField] private Toggle assetsToggle;
        [SerializeField] private Toggle charactersToggle;
        [SerializeField] private GameObject mainUI;
        [SerializeField] private GameObject topMenu;
        [SerializeField] private Transform characterPosition;
        
        private AssetLoader assetLoader;
        private CharacterLoader characterLoader;
        
        private string baseModelId;
        private string selectedCategory;
        private CharacterData characterData;

        private void Start()
        {
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
            EventAggregator.Instance.OnAssetSelected += OnAssetSelected;
            EventAggregator.Instance.OnPageChanged += OnPageChanged;

            assetLoader = new AssetLoader();
            characterLoader = new CharacterLoader();
        }

        public void DisplayUI()
        {
            if(mainUI.activeSelf) return;
            
            mainUI.SetActive(true);
            topMenu.SetActive(false);
            categoryController.LoadCategories(categoriesToggle.isOn);
            
            LoadCharacter();
        }

        private void OnCategorySelected(string category)
        {
            selectedCategory = category;
            assetPageController.LoadAssets(category, baseModelId, assetsToggle.isOn);
        }
        
        private void OnAssetSelected(Asset asset)
        {
            LoadAssetAsync(asset).ConfigureAwait(false);
        }
        
        private void OnPageChanged(int page)
        {
            assetPageController.LoadAssets(selectedCategory, baseModelId, assetsToggle.isOn, page);
        }
        
        private async void LoadCharacter()
        {
            baseModelId = await GetFirstBasemodelId();
            characterData = await characterLoader.LoadCharacter(baseModelId, charactersToggle.isOn);
            PlaceCharacterInScene(characterData);
        }
        
        private void PlaceCharacterInScene(CharacterData characterData)
        {
            characterData.transform.SetParent(characterPosition, false);
        }

        private async Task HandleBaseModelAssetAsync(Asset asset)
        {
            baseModelId = asset.Id;
                
            if (charactersToggle.isOn)
            {
                CharacterData newCharacterData = await characterLoader.LoadCharacter(baseModelId, charactersToggle.isOn);
                    
                foreach (var assetMesh in assetLoader.Assets)
                {
                    await LoadAssetAsync(assetMesh.Value);
                }
                    
                Destroy(characterData.gameObject);
                characterData = newCharacterData;
            }
            else
            {
                CharacterData newCharacterData = await characterLoader.LoadAsyncX(characterData.Id, asset.Id, asset);
                if(characterData != null) Destroy(characterData.gameObject);
                characterData = newCharacterData;
            }
        }

        private async Task HandleCustomizationAssetAsync(Asset asset)
        {
            if (charactersToggle.isOn)
            {
                GameObject assetModel = await assetLoader.GetAssetModelAsync(asset, baseModelId, charactersToggle.isOn);
                characterLoader.SwapAsset(characterData, asset, assetModel);
                assetModel.transform.SetParent(characterPosition, false);
            }
            else
            {
                CharacterData newCharacterData = await characterLoader.LoadAsyncX(characterData.Id, baseModelId, asset);
                if(characterData != null) Destroy(characterData.gameObject);
                characterData = newCharacterData;
            }
        }
        
        private async Task LoadAssetAsync(Asset asset)
        {
            if(asset.Type == "baseModel")
            {
                await HandleBaseModelAssetAsync(asset);
            }
            else
            {
                await HandleCustomizationAssetAsync(asset);
            }
            
            PlaceCharacterInScene(characterData);
        }
        
        private async Task<string> GetFirstBasemodelId()
        {
            AssetListResponse baseModelResponse = await assetLoader.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams()
                {
                    Type = "baseModel",
                }
            }, charactersToggle.isOn);

            return baseModelResponse.Data[0].Id;
        }
    }
}
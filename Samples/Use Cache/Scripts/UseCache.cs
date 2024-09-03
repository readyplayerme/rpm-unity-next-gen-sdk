using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        
        private string selectedCategory;

        private AssetLoader assetLoader;
        private CharacterApi characterApi;
        private CharacterLoader characterLoader;
        
        private string characterId;
        private string baseModelId;
        private CharacterData characterData;

        private void Start()
        {
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
            EventAggregator.Instance.OnAssetSelected += OnAssetSelected;
            EventAggregator.Instance.OnPageChanged += OnPageChanged;

            assetLoader = new AssetLoader();
            characterApi = new CharacterApi();
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
            LoadAsset(asset);
        }
        
        private void OnPageChanged(int page)
        {
            assetPageController.LoadAssets(selectedCategory, baseModelId, assetsToggle.isOn, page);
        }
        
        private async void LoadCharacter()
        {
            baseModelId = await GetFirstBasemodelId();
            characterData = await characterLoader.LoadCharacter(baseModelId, charactersToggle.isOn);
            characterData.transform.SetParent(characterPosition, false);
        }
        
        private async void LoadAsset(Asset asset)
        {
            if(asset.Type == "baseModel")
            {
                Destroy(characterData.gameObject);
                baseModelId = asset.Id;
                
                if (charactersToggle.isOn)
                {
                    characterData = characterLoader.LoadTemplate(asset.Id);
                    
                    foreach (var assetMesh in assetLoader.Assets)
                    {
                        LoadAsset(assetMesh.Value);
                    }
                }
                else
                {
                    if(characterData != null) Destroy(characterData.gameObject);
                    characterData = await characterLoader.LoadAsync(characterId, asset);
                }
            }
            else
            {
                if (charactersToggle.isOn)
                {
                    GameObject assetModel = await assetLoader.GetAssetModelAsync(asset, baseModelId, charactersToggle.isOn);
                    characterLoader.SwapAsset(characterData, asset, assetModel);
                    assetModel.transform.SetParent(characterPosition, false);
                }
                else
                {
                    await characterApi.UpdateAsync(new CharacterUpdateRequest()
                    {
                        Id = characterId,
                        Payload = new CharacterUpdateRequestBody()
                        {
                            Assets = new Dictionary<string, string>
                            {
                                { asset.Type, asset.Id }
                            }
                        }
                    });

                    if(characterData != null) Destroy(characterData.gameObject);
                    characterData = await characterLoader.LoadAsync(characterId);
                }
            }
            
            characterData.gameObject.transform.SetParent(characterPosition, false);
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

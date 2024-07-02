using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadyPlayerMe.Samples.PlayerLocomotion
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [SerializeField] private AssetButton assetButtonPrefab;
        [SerializeField] private ScrollRect baseModelScrollView;
        [SerializeField] private ScrollRect outfitScrollView;
        [SerializeField] private GameObject loadingPanel;
        
        private AssetApi assetApi;
        private CharacterApi characterApi;
        private CharacterManager characterManager;
        
        private string characterId;
        private string baseModelId;
        
        private CharacterData character;
        
        private async void Start()
        {
            assetApi = new AssetApi();
            characterApi = new CharacterApi();
            characterManager = new CharacterManager();
            
            baseModelId = await LoadBaseModels();
            LoadOutfits();

            var createResponse = await characterApi.CreateAsync(new CharacterCreateRequest()
            {
                Payload = new CharacterCreateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { "baseModel", baseModelId }
                    }
                }
            });
            
            characterId = createResponse.Data.Id;
            character = await characterManager.LoadCharacter(characterId, baseModelId);
            
            loadingPanel.SetActive(false);
        }

        private async Task<string> LoadBaseModels()
        {
            AssetListResponse baseModelResponse = await assetApi.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams()
                {
                    Type = "baseModel",
                }
            });

            foreach (var asset in baseModelResponse.Data)
            {
                AssetButton button = Instantiate(assetButtonPrefab, baseModelScrollView.content);
                button.Init(asset, UpdateCharacter);
            }
            
            return baseModelResponse.Data[0].Id;
        }

        private async void LoadOutfits()
        {
            AssetListResponse outfitResponse = await assetApi.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams()
                {
                    Type = "top",
                }
            });

            foreach (var asset in outfitResponse.Data)
            {
                AssetButton button = Instantiate(assetButtonPrefab, outfitScrollView.content);
                button.Init(asset, UpdateCharacter);
            }
        }
        
        private async void UpdateBaseModel(Asset asset)
        {
            loadingPanel.SetActive(true);
            
            await characterApi.UpdateAsync(new CharacterUpdateRequest()
            {
                Id = characterId,
                Payload = new CharacterUpdateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { "baseModel", asset.Id }
                    }
                }
            });
            
            character = await characterManager.LoadCharacter(characterId, asset.Id);
            
            loadingPanel.SetActive(false);
        }

        private async void UpdateCharacter(Asset asset)
        {
            loadingPanel.SetActive(true);
            
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
            
            character = await characterManager.LoadCharacter(characterId);
            
            loadingPanel.SetActive(false);
        }
    }
}

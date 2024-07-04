using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadyPlayerMe.Samples.PlayerLocomotion
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [SerializeField] private AssetButton assetButtonPrefab;
        [SerializeField] private ScrollRect baseModelScrollView;
        [SerializeField] private ScrollRect topsScrollView;
        [SerializeField] private GameObject loadingPanel;
        
        private AssetApi assetApi;
        private CharacterApi characterApi;
        private CharacterManager characterManager;
        
        private string characterId;
        
        private async void Start()
        {
            assetApi = new AssetApi();
            characterApi = new CharacterApi();
            characterManager = new CharacterManager();
            
            string baseModelId = await LoadBaseModels();
            LoadTops();

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
            await characterManager.LoadCharacter(characterId, baseModelId);
            
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
                button.Init(asset, UpdateBaseModel);
            }
            
            return baseModelResponse.Data[0].Id;
        }

        private async void LoadTops()
        {
            AssetListResponse topsResponse = await assetApi.ListAssetsAsync(new AssetListRequest
            {
                Params = new AssetListQueryParams()
                {
                    Type = "top",
                }
            });

            foreach (var asset in topsResponse.Data)
            {
                AssetButton button = Instantiate(assetButtonPrefab, topsScrollView.content);
                button.Init(asset, UpdateTops);
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
            
            await characterManager.LoadCharacter(characterId, asset.Id);
            
            loadingPanel.SetActive(false);
        }

        private async void UpdateTops(Asset asset)
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
            
            await characterManager.LoadCharacter(characterId);
            
            loadingPanel.SetActive(false);
        }
    }
}

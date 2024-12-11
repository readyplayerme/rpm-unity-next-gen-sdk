using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Samples.PlayerLocomotion
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [SerializeField] private AssetButton assetButtonPrefab;
        [SerializeField] private ScrollRect baseModelScrollView;
        [SerializeField] private ScrollRect topsScrollView;
        [SerializeField] private GameObject loadingPanel;
        [Space]
        [SerializeField, Tooltip("If set to null it will fallback DefaultTemplateList if it exists")] private CharacterBlueprintTemplateList characterBlueprintTemplateList;
        
        private AssetLoader assetLoader;
        private CharacterLoader characterLoader;
        
        private string baseModelId;
        private CharacterData characterData;
        
        private async void Start()
        {
            assetLoader = new AssetLoader();
            characterLoader = new CharacterLoader(characterBlueprintTemplateList);
            
            baseModelId = await LoadBaseModels();
            LoadTops();

            characterData = await characterLoader.LoadCharacter(baseModelId);
            
            loadingPanel.SetActive(false);
        }

        #region User Interface
        private async Task<string> LoadBaseModels()
        {
            AssetListResponse baseModelResponse = await assetLoader.ListAssetsAsync(new AssetListRequest
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
            AssetListResponse topsResponse = await assetLoader.ListAssetsAsync(new AssetListRequest
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
        #endregion
        
        private async void UpdateBaseModel(Asset asset)
        {
            baseModelId = asset.Id;
            loadingPanel.SetActive(true);
            
            var operation = characterLoader.LoadAsync(characterData.Id, asset.Id, asset);
            
            await TransferCharacterStateInfo(characterData, operation);
            
            loadingPanel.SetActive(false);
        }

        private async void UpdateTops(Asset asset)
        {
            loadingPanel.SetActive(true);
            
            CharacterData newCharacterData = await characterLoader.LoadAsync(characterData.Id, baseModelId, asset);
            ReplaceCharacter(newCharacterData);
            
            loadingPanel.SetActive(false);
        }

        private async Task TransferCharacterStateInfo(CharacterData data, Task<CharacterData> task)
        {
            CharacterData newCharacterData = await task;

            var animatorState = data.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            var animTime = animatorState.normalizedTime;
            var characterRotation = data.gameObject.transform.rotation;
            var characterPosition = data.gameObject.transform.position;
            
            // set and update character data
            newCharacterData.gameObject.GetComponent<Animator>().Play(animatorState.fullPathHash);
            newCharacterData.gameObject.GetComponent<Animator>().Update(animTime);
            newCharacterData.gameObject.transform.rotation = characterRotation;
            newCharacterData.gameObject.transform.position = characterPosition;
            
            ReplaceCharacter(newCharacterData);
        }
        
        private void ReplaceCharacter(CharacterData data)
        {
            Destroy(characterData.gameObject);
            characterData = data;
        }
    }
}

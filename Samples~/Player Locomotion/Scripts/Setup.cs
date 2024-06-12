using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.CharacterLoader;
using ReadyPlayerMe.Data;

public class Setup : MonoBehaviour
{
    [SerializeField] private string styleId;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private StoreItem storeItemPrefab;

    private CharacterApi _characterApi;
    private CharacterManager _characterManager;
    private string _characterId;
    private GameObject _template;

    private async void Start()
    {
        var settings = Resources.Load<Settings>("ReadyPlayerMeSettings");

        if (string.IsNullOrEmpty(settings.ApiProxyUrl) && string.IsNullOrEmpty(settings.ApiKey))
        {
            Debug.LogError("You need to configure authentication for the SDK before running this sample. See these docs: https://readyplayerme.notion.site/Authenticating-the-SDK-fbf58ab4670243bcb3d8082268426752");
            return;
        }
        
        LoadStore();

        _characterApi = new CharacterApi();
        var assetApi = new AssetApi();
        
        if (string.IsNullOrEmpty(styleId))
        {
            styleId = (await assetApi.ListAssetsAsync(new AssetListRequest()
            {
                Params = new AssetListQueryParams
                {
                    Type = "baseModel"
                }
            })).Data.FirstOrDefault()?.Id;
        }

        var payload = new CharacterCreateRequestBody()
            {
                Assets = new Dictionary<string, string>()
                {
                    { "baseModel", styleId }
                }
            };

        var request = new CharacterCreateRequest()
        {
            Payload = payload
        };
        var response = await _characterApi.CreateAsync(request);

        _characterId = response.Data.Id;
        _characterManager = new CharacterManager();

        await _characterManager.LoadCharacter(_characterId, styleId);
    }

    private async void UpdateOutfit(Asset asset)
    {
        var updateRequest = new CharacterUpdateRequest()
        {
            Id = _characterId,
            Payload = new CharacterUpdateRequestBody()
            {
                Assets = new Dictionary<string, string>
                {
                    { asset.Type, asset.Id }
                }
            }
        };
        var updateResponse = await _characterApi.UpdateAsync(updateRequest);

        await _characterManager.LoadCharacter(updateResponse.Data.Id);
    }

    private async void LoadStore()
    {
        var assetApi = new AssetApi();
        var response = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params = new AssetListQueryParams()
            {
                ExcludeTypes = "baseModel"
            }
        });

        foreach (var asset in response.Data)
        {
            var storeItem = Instantiate(storeItemPrefab, scrollView.content);
            storeItem.Init(asset, UpdateOutfit);
        }
    }
}
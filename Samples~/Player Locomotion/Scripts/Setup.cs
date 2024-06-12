using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;
using ReadyPlayerMe.CharacterLoader;

public class Setup : MonoBehaviour
{
    [SerializeField] private string characterStyleId = "665e05e758e847063761c985";
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private StoreItem storeItemPrefab;

    private CharacterApi _characterApi;
    private CharacterManager _characterManager;
    private string _characterId;
    private GameObject _template;

    private async void Start()
    {
        LoadStore();

        _characterApi = new CharacterApi();

        var payload = string.IsNullOrEmpty(characterStyleId)
            ? new CharacterCreateRequestBody()
            : new CharacterCreateRequestBody()
            {
                Assets = new Dictionary<string, string>()
                {
                    { "baseModel", characterStyleId }
                }
            };

        var request = new CharacterCreateRequest()
        {
            Payload = payload
        };
        var response = await _characterApi.CreateAsync(request);

        _characterId = response.Data.Id;
        _characterManager = new CharacterManager();

        await _characterManager.LoadCharacter(_characterId, characterStyleId);
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
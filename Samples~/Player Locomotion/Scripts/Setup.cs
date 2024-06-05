using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;
using ReadyPlayerMe.CharacterLoader;

public class Setup : MonoBehaviour
{
    [SerializeField] private string templateTag;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private StoreItem storeItemPrefab;

    private CharacterApi _characterApi;
    private CharacterLoader _characterLoader;
    private GameObject _instance;
    private string _characterId;
    private GameObject _template;

    private async void Start()
    {
        LoadStore();

        _characterApi = new CharacterApi();
        
        var request = new CharacterCreateRequest();
        var response = await _characterApi.CreateAsync(request);
        _characterId = response.Data.Id;
        
        _characterLoader = new CharacterLoader();

        await _characterLoader.LoadAsync(_characterId, templateTag);
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
        
        await _characterLoader.LoadAsync(updateResponse.Data.Id, _instance);
    }

    private async void LoadStore()
    {
        var assetApi = new AssetApi();
        var response = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params =  new AssetListQueryParams()
            {
            }
        });

        foreach (var asset in response.Data)
        {
            var storeItem = Instantiate(storeItemPrefab, scrollView.content);
            storeItem.Init(asset, UpdateOutfit);
        }
    }
}

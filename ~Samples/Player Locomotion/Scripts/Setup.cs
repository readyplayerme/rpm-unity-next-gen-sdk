using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader;

public class Setup : MonoBehaviour
{
    [SerializeField] private string baseModelId;
    [SerializeField] private string templateTag;
    
    [SerializeField] private Button storeButton;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private StoreItem storeItemPrefab;

    private AvatarApi avatarApi;
    private AvatarLoader avatarLoader;
    private GameObject instance;
    private string avatarId;
    private GameObject template;
    
    async void Start()
    {
        storeButton.onClick.AddListener(LoadStore);
        
        avatarApi = new AvatarApi();
        var request = new AvatarCreateRequest()
        {
            Payload = new AvatarCreateRequestBody
            {
                ApplicationId = "6628c280ecb07cb9d9cd7238",
                Assets = new Dictionary<string, string>
                {
                    { "baseModel",  baseModelId } //"6644b977fd829d77ca263be2"
                }
            }
        };
        
        var response = await avatarApi.CreateAvatarAsync(request);
        avatarId = response.Data.Id;
        
        avatarLoader = new AvatarLoader();
        
        template = TemplateLoader.GetByTag(templateTag).template;
        instance = Instantiate(template);
        instance.SetActive(false);
        
        await avatarLoader.LoadAsync(avatarId, instance);
        instance.SetActive(true);
    }

    private async void UpdateOutfit(Asset asset)
    {
        var updateRequest = new AvatarUpdateRequest()
        {
            AvatarId = avatarId,
            Payload = new AvatarUpdateRequestBody()
            {
                Assets = new Dictionary<string, string>
                {
                    { asset.Type, asset.Id }
                }
            }
        };
        var updateResponse = await avatarApi.UpdateAvatarAsync(updateRequest);
        
        await avatarLoader.LoadAsync(updateResponse.Data.Id, instance);
    }

    public async void LoadStore()
    {
        AssetApi assetApi = new AssetApi();
        var response = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params =  new AssetListQueryParams()
            {
                ApplicationId = "6628c280ecb07cb9d9cd7238",
                Type = "top"
            }
        });

        foreach (var asset in response.Data)
        {
            var storeItem = Instantiate(storeItemPrefab, scrollView.content);
            storeItem.Init(asset, UpdateOutfit);
        }
    }
}

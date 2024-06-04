using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;
using ReadyPlayerMe.AvatarLoader;

public class Setup : MonoBehaviour
{
    [SerializeField] private string templateTag;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private StoreItem storeItemPrefab;

    private AvatarApi avatarApi;
    private AvatarLoader avatarLoader;
    private GameObject instance;
    private string avatarId;
    private GameObject template;
    
    async void Start()
    {
        LoadStore();

        avatarApi = new AvatarApi();
        var request = new AvatarCreateRequest();

        var response = await avatarApi.CreateAvatarAsync(request);
        avatarId = response.Data.Id;
        
        avatarLoader = new AvatarLoader();
        
        template = TemplateLoader.GetByTag(templateTag)?.template;

        if (!template)
        {
            Debug.LogError("No template found with tag '" + templateTag + "'. Either create one, or sign out, and sign in with the demo account to play this sample.");
            return;
        }

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

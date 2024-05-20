using UnityEngine;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Data;
using System.Collections.Generic;

public class Setup : MonoBehaviour
{
    [SerializeField] private GameObject avatarTemplate;
    [SerializeField] private AvatarSkeletonDefinition definition;

    private AvatarApi avatarApi;
    
    async void Start()
    {
        avatarApi = new AvatarApi();
        var request = new AvatarCreateRequest()
        {
            Payload = new AvatarCreateRequestBody
            {
                ApplicationId = "6628c280ecb07cb9d9cd7238",
                Assets = new Dictionary<string, string>
                {
                    { "baseModel",  "6644b977fd829d77ca263be2"}
                }
            }
        };
        
        var response = await avatarApi.CreateAvatarAsync(request);
        
        AvatarLoader avatarLoader = new AvatarLoader();
        
        var instance = Instantiate(avatarTemplate);
        instance.SetActive(false);
        
        await avatarLoader.LoadAsync(response.Data.Id, instance, definition);
        instance.SetActive(true);
        
        // Update avatar
        var updateRequest = new AvatarUpdateRequest()
        {
            AvatarId = response.Data.Id,
            Payload = new AvatarUpdateRequestBody()
            {
                Assets = new Dictionary<string, string>
                {
                    { "bottom", "6628c3df97cb7a2453b803b2" }
                }
            }
        };
        var updateResponse = await avatarApi.UpdateAvatarAsync(updateRequest);
        
        await avatarLoader.LoadAsync(updateResponse.Data.Id, instance, definition);
    }
}

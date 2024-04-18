using System.Collections.Generic;
using ReadyPlayerMe.Runtime.Api.V1.Assets;
using ReadyPlayerMe.Runtime.Api.V1.Assets.Models;
using ReadyPlayerMe.Runtime.Api.V1.Characters;
using ReadyPlayerMe.Runtime.Api.V1.Characters.Models;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private async void Start()
    {
        var organizationId = "63eba72b442b6965bf9f1ed2";
        
        var assetApi = new AssetApi();
        var characterStyleResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params =
            {
                OrganizationId = organizationId,
                Type = "baseModel"
            }
        });

        Debug.Log($"Number of character styles: {characterStyleResponse.Pagination.TotalDocs}");
        
        var assetListResponse = await assetApi.ListAssetsAsync(new AssetListRequest()
        {
            Params =
            {
                OrganizationId = organizationId,
                ExcludeTypes = "baseModel"
            }
        });

        Debug.Log($"Number of assets: {assetListResponse.Pagination.TotalDocs}");
        
        var characterApi = new CharacterApi();
        var characterCreateResponse = await characterApi.CreateCharacterAsync(new CharacterCreateRequest()
        {
            Payload = new CharacterCreateRequestBody
            {
                OrganizationId = organizationId
            }
        });
        
        Debug.Log($"New character id: {characterCreateResponse.Data.Id}");

        var characterUpdateResponse = await characterApi.UpdateCharacterAsync(new CharacterUpdateRequest()
        {
            CharacterId = characterCreateResponse.Data.Id,
            Payload = new CharacterUpdateRequestBody()
            {
                OrganizationId = organizationId,
                Assets = new Dictionary<string, string>
                {
                    { "baseModel", characterStyleResponse.Data[0].Id },
                    { assetListResponse.Data[0].Type, assetListResponse.Data[0].Id }
                }
            }
        });
        
        Debug.Log($"Updated character url: {characterUpdateResponse.Data.GlbUrl}");
    }
}

/*
{
    "type":"BadRequestError",
    "status":400,
    "message":"Bad Request",
    "data":{
        "query":[{"instancePath":"/assets",
        "schemaPath":"#/properties/assets/type",
        "keyword":"type",
        "params":{"type":"object"},
        "message":"must be object"}]}}
*/
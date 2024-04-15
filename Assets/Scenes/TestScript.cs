using UnityEngine;
using ReadyPlayerMe.Phoenix;
using ReadyPlayerMe.Phoenix.Data;

public class TestScript : MonoBehaviour
{
    private async void Start()
    {
        var baseModel = new BaseModelsEndpoint();
        var res = await baseModel.Get();
        
        var characters = new CharactersEndpoint();
        var res2 = await characters.CreateAvatar(new CharactersRequestData
        {
            organizationBaseModelId = res.data[0].id,
            organizationId = res.data[0].organizationId
        });
        
        // var res3 = await characters.PreviewAssetOnCharacter(res2.data.id, res2.data.id);
        
        var refittedAssets = new RefittedAssetEndpoint();
        var res4 = await refittedAssets.ListEquipableAssets();
        Debug.Log($"Number of assets: {res4.data.Length}");
        
        var res5 = await refittedAssets.ListEquipableAssets("646e76caa993eb77d5a2b831");
        Debug.Log($"Number of assets in organization [{res2.data.organizationId}]: {res5.data.Length}");
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
using System.Collections.Generic;
using System.IO;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor
{
    public static class CharacterTemplateCreator
    {
        private static string RPM_RESOURCES_PATH = "Assets/Ready Player Me/Resources";
        public static string DEFAULT_TEMPLATES_LIST_ASSET = "CharacterBlueprintTemplateList";
        
        public static async void LoadAndCreateTemplateList(string applicationId)
        {
            var blueprintApi = new BlueprintApi();
            var blueprints = await blueprintApi.ListAsync(new BlueprintListRequest
            {
                ApplicationId = applicationId
            });
            var fileApi = new FileApi();
            var templates = new List<CharacterBlueprintTemplate>();
            
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me"))
                AssetDatabase.CreateFolder("Assets", "Ready Player Me"); 
            
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me/Templates"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Templates"); 
            
            foreach (var blueprint in blueprints.Data)
            {
                var template = ScriptableObject.CreateInstance<CharacterBlueprintTemplate>();
                template.name = blueprint.Name;
                template.id = blueprint.Id;
                template.cacheId = blueprint.Id ;
                var bytes = await fileApi.DownloadFileIntoMemoryAsync(blueprint.CharacterModel.ModelUrl);
                var path = $"Assets/Ready Player Me/Templates/{blueprint.Id}.glb";
                await File.WriteAllBytesAsync(path, bytes);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                var loadedAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (loadedAsset == null)
                {
                    Debug.Log($"Asset not found at {path}");
                    continue;
                }
                var blueprintPrefab = new BlueprintPrefab();
                blueprintPrefab.prefab = loadedAsset;
                blueprintPrefab.tags = new List<string> {""};
                template.prefabs = new[] { blueprintPrefab };
                templates.Add(template);
                AssetDatabase.CreateAsset(template, $"Assets/Ready Player Me/Templates/Template_{blueprint.Id}.asset");
                Debug.Log($"Created template {blueprint.Name} ");
            }
            
            var templateListObject = ScriptableObject.CreateInstance<CharacterBlueprintTemplateList>();
            templateListObject.templates = templates.ToArray();

            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me/Resources"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Resources");
            
            // Create or update the template list asset
            if (AssetDatabase.LoadAssetAtPath<CharacterBlueprintTemplateList>($"{RPM_RESOURCES_PATH}/{DEFAULT_TEMPLATES_LIST_ASSET}.asset") != null)
            {
                AssetDatabase.DeleteAsset($"{RPM_RESOURCES_PATH}/{DEFAULT_TEMPLATES_LIST_ASSET}.asset");
            }
            AssetDatabase.CreateAsset(templateListObject, $"{RPM_RESOURCES_PATH}/{DEFAULT_TEMPLATES_LIST_ASSET}.asset");
            AssetDatabase.SaveAssetIfDirty(templateListObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log( "Templates created" );
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor
{
    public static class CharacterTemplateCreator
    {
        private static string RPM_RESOURCES_PATH = "Assets/Ready Player Me/Resources";
        public static string DEFAULT_TEMPLATES_LIST_ASSET = "DefaultTemplateList";
        
        public static async Task LoadAndCreateTemplateList(string applicationId)
        {
            ValidateFolders();
            var blueprints = await GetBlueprints(applicationId);
            var templateListObject = await LoadAndCreateBlueprintTemplateList(blueprints);
            
            // Create or update the template list asset
            if (AssetDatabase.LoadAssetAtPath<CharacterTemplateList>($"{RPM_RESOURCES_PATH}/{DEFAULT_TEMPLATES_LIST_ASSET}.asset") != null)
            {
                AssetDatabase.DeleteAsset($"{RPM_RESOURCES_PATH}/{DEFAULT_TEMPLATES_LIST_ASSET}.asset");
            }
            AssetDatabase.CreateAsset(templateListObject, $"{RPM_RESOURCES_PATH}/{DEFAULT_TEMPLATES_LIST_ASSET}.asset");
            AssetDatabase.SaveAssetIfDirty(templateListObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log( "Templates created" );
        }

        private static async Task<CharacterTemplateList> LoadAndCreateBlueprintTemplateList(CharacterBlueprint[] blueprints)
        {
            var fileApi = new FileApi();
            var templates = new List<CharacterTemplate>();
            foreach (var blueprint in blueprints)
            {
                var template = ScriptableObject.CreateInstance<CharacterTemplate>();
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
            
            var templateListObject = ScriptableObject.CreateInstance<CharacterTemplateList>();
            templateListObject.templates = templates.ToArray();
            return templateListObject;
        }

        private static async Task<CharacterBlueprint[]> GetBlueprints(string applicationId)
        {
            var blueprintApi = new BlueprintApi();
            var blueprints = await blueprintApi.ListAsync(new BlueprintListRequest
            {
                ApplicationId = applicationId
            });
            return blueprints.Data;
        }

        private static void ValidateFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me"))
                AssetDatabase.CreateFolder("Assets", "Ready Player Me"); 
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me/Templates"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Templates"); 
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me/Resources"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Resources");
        }
    }
}
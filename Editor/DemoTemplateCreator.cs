using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Cache;
using UnityEditor;
using UnityEngine;
using CharacterTemplateConfig = ReadyPlayerMe.Data.CharacterTemplateConfig;

namespace ReadyPlayerMe.Editor
{
    public static class CharacterTemplateCreator
    {
        private const string RPM_RESOURCES_PATH = "Assets/Ready Player Me/Resources";

        public static async Task LoadAndCreateTemplateList(string applicationId)
        {
            ValidateFolders();
            if (string.IsNullOrEmpty(applicationId)) return;
            var templateListObject = AssetDatabase.LoadAssetAtPath<CharacterTemplateConfig>($"{RPM_RESOURCES_PATH}/{applicationId}.asset");
            if (templateListObject != null && templateListObject.Templates is { Length: > 0 }) {
                return; // config for this application already exists return so we dont overwrite settings
            }

            templateListObject = ScriptableObject.CreateInstance<CharacterTemplateConfig>();
            AssetDatabase.CreateAsset(templateListObject, $"{RPM_RESOURCES_PATH}/{applicationId}.asset");
            var blueprints = await GetBlueprints(applicationId);
            var templateList = await LoadAndCreateCharacterTemplates(blueprints);
            
            templateListObject.Templates = templateList;
            AssetDatabase.SaveAssetIfDirty(templateListObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log( $"Template config created with {templateListObject.Templates.Length} templates." );
        }

        private static async Task<CharacterTemplate[]> LoadAndCreateCharacterTemplates(CharacterBlueprint[] blueprints)
        {
           
            var templates = new List<CharacterTemplate>();
            foreach (var blueprint in blueprints)
            {
                var template = new CharacterTemplate();
                template.Name = blueprint.Name;
                template.ID = blueprint.Id;
                template.CacheId = blueprint.Id ;
                //TODO: similar logic for loading and storing .glb is in GlbCache.cs, need to revisit or refactor in future
                var loadedAsset = await LoadBlueprintModel(blueprint);
                if (loadedAsset == null)
                {
                    continue;
                }
                var blueprintPrefab = new BlueprintPrefab();
                blueprintPrefab.Prefab = loadedAsset;
                blueprintPrefab.Tags = new string[] {"Default"};
                template.Prefabs = new[] { blueprintPrefab };
                templates.Add(template);
            }
            return templates.ToArray();
        }

        private static async Task<GameObject> LoadBlueprintModel(CharacterBlueprint blueprint)
        {
            var fileApi = new FileApi();
            var bytes = await fileApi.DownloadFileIntoMemoryAsync(blueprint.CharacterModel.ModelUrl);
            var glbCache = new GlbCache("Character Blueprints");
            await glbCache.Save(bytes, blueprint.Id);
            var loadedAsset = glbCache.Load(blueprint.Id);
            return loadedAsset;
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
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me/Resources"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Resources");
        }
    }
}
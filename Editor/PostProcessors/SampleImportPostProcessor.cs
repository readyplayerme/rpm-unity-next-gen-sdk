using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using ReadyPlayerMe.Editor.Api.V1.Analytics.Models;
using ReadyPlayerMe.Editor.Cache;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.PostProcessors
{
    public class SampleImportPostProcessor : AssetPostprocessor
    {
        public const string DemoCharacterBlueprintId = "665e05e758e847063761c985";
        
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
            )
        {
            // TODO re-implement or remove
            // if (importedAssets.FirstOrDefault(p => p.Contains("Character Clothing Example")) != null)
            // {
            //     var characterBlueprintConfigCache = new ScriptableObjectCache<CharacterBlueprintTemplateList>();
            //     var characterBlueprintConfig = characterBlueprintConfigCache.Init(CharacterTemplateList.AssetName);
            //     var templates = characterBlueprintConfig.templates?.ToList() ?? new List<CharacterTemplate>();
            //     var existingTemplate = templates.FirstOrDefault(p => p.tags.Contains(DemoCharacterBlueprintId));
            //     if (existingTemplate == null)
            //     {
            //         var matchingTemplates = AssetDatabase.FindAssets("RPM_Character_Template");
            //         var templatePath = AssetDatabase.GUIDToAssetPath(matchingTemplates[0]);
            //         var templateAsset = AssetDatabase.LoadAssetAtPath<GameObject>(templatePath);
            //
            //         templates.Add(new CharacterBlueprintTemplate()
            //         {
            //             id = Guid.NewGuid().ToString(),
            //             cacheId = matchingTemplates[0],
            //             tags = new List<string>() { DemoCharacterBlueprintId },
            //             prefab = templateAsset
            //         });
            //     }
            //
            //     characterBlueprintConfig.templates = templates.ToArray();
            //     
            //     EditorUtility.SetDirty(characterBlueprintConfig);
            //     AssetDatabase.SaveAssets();
            //     AssetDatabase.Refresh();
            //
            //     var analyticsApi = new AnalyticsApi();
            //     analyticsApi.SendEvent(new AnalyticsEventRequest()
            //     {
            //         Payload = new AnalyticsEventRequestBody()
            //         {
            //             Event = "next gen unity sdk action",
            //             Properties =
            //             {
            //                 { "type", "Custom Clothing Sample Imported" }
            //             }
            //         }
            //     });
            //}
        }
    }
}
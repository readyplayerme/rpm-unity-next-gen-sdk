using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.PostProcessors
{
    public class SampleImportPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
            )
        {
            if (importedAssets.FirstOrDefault(p => p.Contains("Character Clothing Example")) != null)
            {
                var characterStyleConfig = Resources.Load<CharacterStyleTemplateConfig>("CharacterStyleTemplateConfig");
                var templates = characterStyleConfig.templates?.ToList() ?? new List<CharacterStyleTemplate>();
                var existingTemplate = templates.FirstOrDefault(p => p.tags.Contains("RPM"));
                if (existingTemplate == null)
                {
                    var matchingTemplates = AssetDatabase.FindAssets("RPM_Character_Template");
                    var templatePath = AssetDatabase.GUIDToAssetPath(matchingTemplates[0]);
                    var templateAsset = AssetDatabase.LoadAssetAtPath<GameObject>(templatePath);

                    templates.Add(new CharacterStyleTemplate()
                    {
                        id = Guid.NewGuid().ToString(),
                        cacheId = matchingTemplates[0],
                        tags = new List<string>() { "RPM" },
                        template = templateAsset
                    });
                }
            
                characterStyleConfig.templates = templates.ToArray();
                
                EditorUtility.SetDirty(characterStyleConfig);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
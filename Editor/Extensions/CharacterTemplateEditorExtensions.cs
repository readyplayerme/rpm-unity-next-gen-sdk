using System.Linq;
using PlayerZero.Data;
using UnityEditor;
using UnityEngine;

namespace PlayerZero.Editor
{
    public static class CharacterTemplateEditorExtensions
    {
        /// <summary>
        /// Gets the GUID of the CharacterTemplate by ID.
        /// </summary>
        public static string GetTemplatePrefabGUID(this CharacterTemplateConfig characterTemplateConfig, string templateId)
        {
            if (string.IsNullOrEmpty(templateId) || characterTemplateConfig == null || characterTemplateConfig.Templates == null)
                return null;
            var template = characterTemplateConfig.Templates.FirstOrDefault(p => p.BlueprintId == templateId);
            if (template == null)
            {
                Debug.LogWarning($"Template with ID {templateId} not found.");
                return null;
            }
            var prefab = template.GetPrefabByTag("");
            var path = AssetDatabase.GetAssetPath(prefab);
            var guid = AssetDatabase.AssetPathToGUID(path);

            return guid;
        }
    }
}
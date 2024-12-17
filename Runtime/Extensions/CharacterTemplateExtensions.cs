using System.Linq;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class CharacterTemplateExtensions
    {
        public static CharacterTemplate GetTemplate(this CharacterTemplateConfig characterTemplateConfig, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;
            var template = characterTemplateConfig.Templates.FirstOrDefault(p => p.BlueprintId == templateId);
            return template;
        }
        
        public static GameObject GetTemplatePrefab(this CharacterTemplateConfig characterTemplateConfig, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;
            var template = characterTemplateConfig.Templates.FirstOrDefault(p => p.BlueprintId == templateId);
            return template?.GetPrefabByTag("");
        }
        
        public static GameObject GetTemplatePrefab(this CharacterTemplateConfig characterTemplateConfig, string templateId, string tag)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;
            
            var template = characterTemplateConfig.Templates.FirstOrDefault(p => p.BlueprintId == templateId);
            return template?.GetPrefabByTag(tag);
        }
        
        /// <summary>
        /// Searches for a prefab in the blueprint that has a specific tag. 
        /// If no prefab with the tag is found, it returns the first prefab in the list.
        /// </summary>
        /// <param name="template">The CharacterBlueprintTemplate to search.</param>
        /// <param name="tag">The tag to search for in the prefab list.</param>
        /// <returns>Returns the GameObject prefab that matches the tag, or the first prefab if no tag is matched.</returns>
        public static GameObject GetPrefabByTag(this CharacterTemplate template, string tag)
        {
            if (template?.Prefabs == null || template.Prefabs.Length == 0)
            {
                Debug.LogWarning("BlueprintTemplate or its prefab list is null/empty.");
                return null;
            }

            // Search for a prefab with a matching tag
            var matchingPrefab = template.Prefabs
                .FirstOrDefault(bp => bp.Tags != null && bp.Tags.Contains(tag))?.Prefab;

            // If no prefab with the tag was found, fall back to the first prefab in the list
            if (matchingPrefab != null) return matchingPrefab;
            if (tag != "")
            {
                Debug.LogWarning($"No prefab found with tag '{tag}', defaulting to the first prefab.");
            }
            matchingPrefab = template.Prefabs[0].Prefab;

            return matchingPrefab;
        }
    }
}
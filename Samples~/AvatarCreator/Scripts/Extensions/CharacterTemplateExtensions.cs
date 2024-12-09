using System.Linq;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public static class CharacterTemplateExtensions
    {
        public static GameObject GetTemplate(this CharacterBlueprintTemplateConfig characterBlueprintTemplateConfig, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;

            return characterBlueprintTemplateConfig.templates.FirstOrDefault(p => p.id == templateId)?
                .template;
        }
        
        public static GameObject GetTemplate(this CharacterBlueprintTemplateConfig characterBlueprintTemplateConfig, string templateId, string tag)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;
            
            var templatesWithId = characterBlueprintTemplateConfig.templates.Where(p => p.id == templateId).ToList();
            if (templatesWithId.Count == 0)
                return null;
            
            if (string.IsNullOrEmpty(tag) || templatesWithId.All(p => p.tags == null || !p.tags.Contains(tag)))
            {
                return templatesWithId.First().template;
            }

            // Return the first template with a matching tag
            return templatesWithId.FirstOrDefault(p => p.tags != null && p.tags.Contains(tag))?.template;
        }
    }
}
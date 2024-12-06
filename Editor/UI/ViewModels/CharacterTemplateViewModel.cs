using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterTemplateViewModel
    {
        public CharacterBlueprintTemplate CharacterBlueprintTemplate;
        public bool IsOpen = false;
        
        public void Init(CharacterBlueprintTemplate characterBlueprintTemplate)
        {
            CharacterBlueprintTemplate = characterBlueprintTemplate;
        }

        public void SaveTemplate(GameObject newTemplate)
        {
            CharacterBlueprintTemplate.template = newTemplate;
            CharacterBlueprintTemplate.cacheId = Cache.Cache.FindAssetGuid(newTemplate);
        }

        public void SaveTag(string tag)
        {
            CharacterBlueprintTemplate.tags[0] = tag;
        }
    }
}
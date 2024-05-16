using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterTemplateViewModel
    {
        public CharacterStyleTemplate CharacterStyleTemplate;
        public bool IsOpen = false;
        
        public void Init(CharacterStyleTemplate characterStyleTemplate)
        {
            CharacterStyleTemplate = characterStyleTemplate;
        }

        public void SaveTemplate(GameObject newTemplate)
        {
            CharacterStyleTemplate.template = newTemplate;
            CharacterStyleTemplate.cacheId = Cache.Cache.FindAssetGuid(newTemplate);
        }

        public void SaveTag(string tag)
        {
            CharacterStyleTemplate.tags[0] = tag;
        }
    }
}
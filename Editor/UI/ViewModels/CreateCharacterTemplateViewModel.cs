using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CreateCharacterTemplateViewModel
    {
        public CharacterStyleTemplate Template = new CharacterStyleTemplate();

        public string Error = string.Empty;
        public string Tag = string.Empty;
        
        public CreateCharacterTemplateViewModel()
        {
        }

        public void Create()
        {
            Error = string.Empty;
            
            if (Template.template == null)
            {
                Error = "Template must be set.";
                return;
            }

            Template.id = GUID.Generate().ToString();
            Template.cacheId = Cache.Cache.FindAssetGuid(Template.template);
            Template.tags = new List<string>()
            {
                Tag
            };

            var templateConfig = Resources.Load<CharacterStyleTemplateConfig>("CharacterStyleTemplateConfig");
            var templateList = templateConfig.templates.ToList();
            
            templateList.Add(Template);
            templateConfig.templates = templateList.ToArray();
            EditorUtility.SetDirty(templateConfig);
            AssetDatabase.Refresh();
        }
    }
}
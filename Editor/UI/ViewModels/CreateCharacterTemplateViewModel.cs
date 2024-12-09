using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using ReadyPlayerMe.Editor.Api.V1.Analytics.Models;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CreateCharacterTemplateViewModel
    {
        private readonly AnalyticsApi _analyticsApi;
        
        public CharacterBlueprintTemplate Template = new CharacterBlueprintTemplate();

        public string Error = string.Empty;
        public string Tag = string.Empty;

        public CreateCharacterTemplateViewModel(AnalyticsApi analyticsApi)
        {
            _analyticsApi = analyticsApi;
        }

        public void Create()
        {
            Error = string.Empty;
            
            if (Template.template == null)
            {
                Error = "Template must be set.";
                return;
            }

            var newTemplate = new CharacterBlueprintTemplate();
            newTemplate.template = Template.template;
            newTemplate.id = Guid.NewGuid().ToString();
            newTemplate.cacheId = Cache.Cache.FindAssetGuid(Template.template);
            newTemplate.tags = new List<string>()
            {
                Tag
            };

            var templateConfig = Resources.Load<CharacterBlueprintTemplateConfig>("CharacterBlueprintTemplateConfig");
            var templateList = templateConfig.templates.ToList();
            templateList.Add(newTemplate);
            templateConfig.templates = templateList.ToArray();
            
            EditorUtility.SetDirty(templateConfig);
            AssetDatabase.Refresh();
            
            _analyticsApi.SendEvent(new AnalyticsEventRequest()
            {
                Payload = new AnalyticsEventRequestBody()
                {
                    Event = "next gen unity sdk action",
                    Properties =
                    {
                        { "type", "Save Template" },
                        { "tag", Tag }
                    }
                }
            });
        }
    }
}
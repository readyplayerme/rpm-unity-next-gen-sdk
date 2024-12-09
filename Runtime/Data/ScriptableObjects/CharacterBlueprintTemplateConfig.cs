using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe.Data
{
    public class CharacterBlueprintTemplateConfig : ScriptableObject
    {
        public CharacterBlueprintTemplate[] templates;
    }

    [Serializable]
    public class CharacterBlueprintTemplate
    {
        public string cacheId;
        
        public GameObject template;

        public List<string> tags;

        public string id;
    }
}

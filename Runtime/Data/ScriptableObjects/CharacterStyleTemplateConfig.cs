using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe.Data
{
    public class CharacterStyleTemplateConfig : ScriptableObject
    {
        public CharacterStyleTemplate[] templates = Array.Empty<CharacterStyleTemplate>();
    }

    [Serializable]
    public class CharacterStyleTemplate
    {
        public string cacheId;
        
        public GameObject template;

        public List<string> tags;

        public string id;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe.Data
{
    public class CharacterStyleTemplateConfig : ScriptableObject
    {
        public List<CharacterStyleTemplate> templates = new List<CharacterStyleTemplate>();
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

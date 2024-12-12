using System;
using UnityEngine;

namespace ReadyPlayerMe.Data
{
    public class CharacterTemplateConfig : ScriptableObject
    {
        public CharacterTemplate[] Templates;
    }
    
    [Serializable]
    public class CharacterTemplate 
    {
        public string Name;
        // TODO: Hide later if we don't want this to be editable in inspector
        public string ID;
        [HideInInspector] // TODO: remove this later once we remove caching
        public string CacheId;

        public BlueprintPrefab[] Prefabs;
    }
    
    [Serializable]
    public class BlueprintPrefab
    {
        public GameObject Prefab;

        public string[] Tags;
    }
}

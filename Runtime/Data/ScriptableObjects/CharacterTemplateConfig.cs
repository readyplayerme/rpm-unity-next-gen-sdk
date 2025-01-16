using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerZero.Data
{
    public class CharacterTemplateConfig : ScriptableObject
    {
        public CharacterTemplate[] Templates;
    }
    
    [Serializable]
    public class CharacterTemplate 
    {
        public string Name;
        
        [ReadOnly] 
        [SerializeField]
        private string blueprintId;
        
        public string BlueprintId => blueprintId;
        
        [FormerlySerializedAs("CacheId"),HideInInspector] // TODO: remove this later once we remove caching
        public string cacheBlueprintId;
      
        public BlueprintPrefab[] Prefabs;
        
        public CharacterTemplate(string name, string blueprintId)
        {
            this.blueprintId = blueprintId;
            Name = name;
            cacheBlueprintId = blueprintId;
        }
    }
    
    [Serializable]
    public class BlueprintPrefab
    {
        public GameObject Prefab;

        public string[] Tags;
    }
}

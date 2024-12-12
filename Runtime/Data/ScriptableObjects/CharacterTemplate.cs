using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe.Data
{
    [Serializable, CreateAssetMenu(fileName = "CharacterTemplate", menuName = "Ready Player Me/CharacterTemplate")]
    public class CharacterTemplate : ScriptableObject
    {
        public string name;
        
        public string id;

        public string cacheId;

        public BlueprintPrefab[] prefabs;
    }
    
    [Serializable]
    public class BlueprintPrefab
    {
        public GameObject prefab;

        public List<string> tags;
    }
}
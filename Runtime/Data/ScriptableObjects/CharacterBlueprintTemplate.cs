using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReadyPlayerMe.Data
{
    [Serializable, CreateAssetMenu(fileName = "CharacterBlueprintTemplate", menuName = "Ready Player Me/CharacterBlueprintTemplate")]
    public class CharacterBlueprintTemplate : ScriptableObject
    {
        public string name;
        
        public string id;

        public string cacheId;

        [FormerlySerializedAs("blueprintPrefabs")] public BlueprintPrefab[] prefabs;
    }
    
    [Serializable]
    public class BlueprintPrefab
    {
        public GameObject prefab;

        public List<string> tags;
    }
}
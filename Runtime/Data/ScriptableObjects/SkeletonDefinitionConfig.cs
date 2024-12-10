using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReadyPlayerMe.Data
{
    public class SkeletonDefinitionConfig : ScriptableObject
    {
        public SkeletonDefinitionLink[] definitionLinks = Array.Empty<SkeletonDefinitionLink>();
    }
    
    [Serializable]
    public class SkeletonDefinitionLink
    {
        public string definitionCacheId;
        
        public SkeletonDefinition definition;

        public string characterBlueprintId;
    }
}
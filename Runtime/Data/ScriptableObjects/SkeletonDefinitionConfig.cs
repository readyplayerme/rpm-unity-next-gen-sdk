using System;
using UnityEngine;

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

        public string characterStyleId;
    }
}
using System;
using UnityEngine;

namespace ReadyPlayerMe.Data
{
    public class AvatarSkeletonDefinitionConfig : ScriptableObject
    {
        public AvatarSkeletonDefinitionLink[] definitionLinks = Array.Empty<AvatarSkeletonDefinitionLink>();
    }
    
    [Serializable]
    public class AvatarSkeletonDefinitionLink
    {
        public string definitionCacheId;
        
        public AvatarSkeletonDefinition definition;

        public string characterStyleId;
    }
}
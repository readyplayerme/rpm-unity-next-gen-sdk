using UnityEngine;
using ReadyPlayerMe.Runtime.Loader;

namespace ReadyPlayerMe.Runtime.Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AvatarSkeletonDefinition", menuName = "Ready Player Me/Avatar Skeleton Definition", order = 1)]
    public class AvatarSkeletonDefinition : ScriptableObject
    {
        public string rootBonePath;
        public SkeletonBuilder.BoneGroup[] boneGroups;
    }
}
using System.Collections.Generic;
using UnityEngine;


namespace ReadyPlayerMe
{
    [CreateAssetMenu(fileName = "CharacterLoaderSettings", menuName = "Ready Player Me/Character Loader Settings", order = 2)]
    public class CharacterLoaderSettings : ScriptableObject
    {
        [Tooltip("The mesh level of detail.")]
        public Lod Lod;

        [Tooltip("The resting pose of the avatars skeleton.")]
        public Pose Pose = Pose.TPose;

        [Tooltip("If set to NONE the mesh, materials and textures will not be combined into 1. (or 2 if an assets texture contains transparency)")]
        public TextureAtlas TextureAtlas;

        [Range(256, 1024), Tooltip("If set to none the mesh, materials and textures will not be combined into 1. (2 if an assets texture contains transparency)")]
        public int TextureSizeLimit = 1024;

        public List<string> MorphTargets = new List<string>
        {
            "none",
        };
    }
}
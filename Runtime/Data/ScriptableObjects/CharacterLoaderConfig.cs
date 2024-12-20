using System.Collections.Generic;
using UnityEngine;


namespace ReadyPlayerMe
{
    /// <summary>
    ///     This enumeration describes the avatar mesh LOD (Level of Detail) options.
    /// </summary>
    public enum MeshLod
    {
        [InspectorName("High (LOD0)")]
        High,
        [InspectorName("Medium (LOD1)")]
        Medium,
        [InspectorName("Low (LOD2)")]
        Low
    }

    /// <summary>
    ///     This enumeration describes the TextureAtlas setting options.
    /// </summary>
    /// <remarks>If set to <c>None</c> the avatar meshes, materials and textures will NOT be combined.</remarks>
    public enum TextureAtlas
    {
        None,
        [InspectorName("High (1024)")]
        High,
        [InspectorName("Medium (512)")]
        Medium,
        [InspectorName("Low (256)")]
        Low
    }
    
    public enum TextureQuality
    {
        High,
        Medium,
        Low
    }
    
    
    [CreateAssetMenu(fileName = "CharacterLoaderSettings", menuName = "Ready Player Me/Character Loader Settings", order = 2)]
    public class CharacterLoaderConfig : ScriptableObject
    {
        [Tooltip("The mesh level of detail.")]
        public MeshLod MeshLod;

        [Tooltip("If set to NONE the mesh, materials and textures will not be combined into 1. (or 2 if an assets texture contains transparency)")]
        public TextureAtlas TextureAtlas;
        
        public TextureQuality TextureQuality;

        [Range(256, 1024), Tooltip("If set to none the mesh, materials and textures will not be combined into 1. (2 if an assets texture contains transparency)")]
        public int TextureSizeLimit = 1024;

        public List<string> MorphTargets = new List<string>
        {
            "none",
        };

        public List<string> MorphTargetsGroup = new List<string>();
    }
}
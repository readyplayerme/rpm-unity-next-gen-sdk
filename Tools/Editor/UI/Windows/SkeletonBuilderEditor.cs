using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Runtime.Loader;
using ReadyPlayerMe.Runtime.Data.ScriptableObjects;

namespace ReadyPlayerMe.Tools.Editor.UI.Windows
{
    public class SkeletonBuilderEditor : EditorWindow
    {
        private Transform avatar;
        private AvatarSkeletonDefinition skeletonDefinition;

        [MenuItem("Tools/Skeleton Builder")]
        public static void Generate()
        {
            var window = GetWindow<SkeletonBuilderEditor>("Skeleton Builder");
            window.minSize = new Vector2(320, 120);
        }

        private void OnGUI()
        {
            avatar = EditorGUILayout.ObjectField("Avatar", avatar, typeof(Transform), true) as Transform;
            skeletonDefinition = EditorGUILayout.ObjectField("Definition", skeletonDefinition, typeof(AvatarSkeletonDefinition), true) as AvatarSkeletonDefinition;
            
            if (GUILayout.Button("Test"))
            {
                var animator = avatar.GetComponent<Animator>();
                var skeleletonBuilder = new SkeletonBuilder();
                HumanDescription description = skeleletonBuilder.CreateHumanDescription(avatar.gameObject, skeletonDefinition.GetHumanBones());
                Avatar animAvatar = AvatarBuilder.BuildHumanAvatar(avatar.gameObject, description);
                avatar.name = avatar.gameObject.name;
                animator.avatar = animAvatar;
                
                AssetDatabase.CreateAsset(animAvatar, "Assets/Avatar.asset");
            }
        }
    }
}

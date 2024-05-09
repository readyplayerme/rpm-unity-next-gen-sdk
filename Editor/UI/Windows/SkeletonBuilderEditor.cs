using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Windows
{
    public class SkeletonBuilderEditor : EditorWindow
    {
        private Transform _avatar;
        private AvatarSkeletonDefinition _skeletonDefinition;

        [MenuItem("Tools/Ready Player Me/Skeleton Builder")]
        public static void Generate()
        {
            var window = GetWindow<SkeletonBuilderEditor>("Skeleton Builder");
            window.minSize = new Vector2(320, 120);
        }

        private void OnGUI()
        {
            _avatar = EditorGUILayout.ObjectField("Avatar", _avatar, typeof(Transform), true) as Transform;
            _skeletonDefinition = EditorGUILayout.ObjectField("Definition", _skeletonDefinition, typeof(AvatarSkeletonDefinition), true) as AvatarSkeletonDefinition;
            
            if (GUILayout.Button("Test"))
            {
                var animator = _avatar.GetComponent<Animator>();
                var skeletonBuilder = new SkeletonBuilder();
                HumanDescription description = skeletonBuilder.CreateHumanDescription(_avatar.gameObject, _skeletonDefinition.GetHumanBones());
                Avatar animAvatar = AvatarBuilder.BuildHumanAvatar(_avatar.gameObject, description);
                _avatar.name = _avatar.gameObject.name;
                animator.avatar = animAvatar;
                
                AssetDatabase.CreateAsset(animAvatar, "Assets/Avatar.asset");
            }
        }
    }
}

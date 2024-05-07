using ReadyPlayerMe.Runtime.Data.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Runtime.Loader;
using Bone = ReadyPlayerMe.Runtime.Loader.SkeletonBuilder.Bone;
using BoneGroup = ReadyPlayerMe.Runtime.Loader.SkeletonBuilder.BoneGroup;

namespace ReadyPlayerMe.Tools.Editor.UI.Windows
{
    public class SkeletonBuilderEditor : EditorWindow
    {
        private Transform targetAvatar;
        private AvatarSkeletonDefinition definition;

        private Transform avatar;
        private Transform skeletonRoot;

        private static BoneGroup[] BoneGroups =
        {
            new BoneGroup() {
                GroupName = "Left Leg", 
                Bones = new Bone[]
                {
                    new Bone() { Name = "LeftUpperLeg", Transform = null },
                    new Bone() { Name = "LeftLowerLeg", Transform = null },
                    new Bone() { Name = "LeftFoot", Transform = null },
                    new Bone() { Name = "LeftToes", Transform = null },
                },
            },
            new BoneGroup() {
                GroupName = "Right Leg", 
                Bones = new Bone[]
                {
                    new Bone() { Name = "RightUpperLeg", Transform = null },
                    new Bone() { Name = "RightLowerLeg", Transform = null },
                    new Bone() { Name = "RightFoot", Transform = null },
                    new Bone() { Name = "RightToes", Transform = null },
                },
            },
            new BoneGroup()
            {
                GroupName  = "Left Hand",
                Bones = new Bone[]
                {
                    new Bone() { Name = "Left Index Distal", Transform = null },
                    new Bone() { Name = "Left Index Intermediate", Transform = null },
                    new Bone() { Name = "Left Index Proximal", Transform = null },
                    new Bone() { Name = "Left Little Distal", Transform = null },
                    new Bone() { Name = "Left Little Intermediate", Transform = null },
                    new Bone() { Name = "Left Little Proximal", Transform = null },
                    new Bone() { Name = "Left Middle Distal", Transform = null },
                    new Bone() { Name = "Left Middle Intermediate", Transform = null },
                    new Bone() { Name = "Left Middle Proximal", Transform = null },
                    new Bone() { Name = "Left Ring Distal", Transform = null },
                    new Bone() { Name = "Left Ring Intermediate", Transform = null },
                    new Bone() { Name = "Left Ring Proximal", Transform = null },
                    new Bone() { Name = "Left Thumb Distal", Transform = null },
                    new Bone() { Name = "Left Thumb Intermediate", Transform = null },
                    new Bone() { Name = "Left Thumb Proximal", Transform = null },
                },
            },
            new BoneGroup() {
                GroupName = "Left Arm", 
                Bones = new Bone[]
                {
                    new Bone() { Name = "LeftShoulder", Transform = null },
                    new Bone() { Name = "LeftUpperArm", Transform = null },
                    new Bone() { Name = "LeftLowerArm", Transform = null },
                    new Bone() { Name = "LeftHand", Transform = null },
                },
            },
            new BoneGroup()
            {
                GroupName = "Right Hand",
                Bones = new Bone[]
                {
                    new Bone() { Name = "Right Index Distal", Transform = null },
                    new Bone() { Name = "Right Index Intermediate", Transform = null },
                    new Bone() { Name = "Right Index Proximal", Transform = null },
                    new Bone() { Name = "Right Little Distal", Transform = null },
                    new Bone() { Name = "Right Little Intermediate", Transform = null },
                    new Bone() { Name = "Right Little Proximal", Transform = null },
                    new Bone() { Name = "Right Middle Distal", Transform = null },
                    new Bone() { Name = "Right Middle Intermediate", Transform = null },
                    new Bone() { Name = "Right Middle Proximal", Transform = null },
                    new Bone() { Name = "Right Ring Distal", Transform = null },
                    new Bone() { Name = "Right Ring Intermediate", Transform = null },
                    new Bone() { Name = "Right Ring Proximal", Transform = null },
                    new Bone() { Name = "Right Thumb Distal", Transform = null },
                    new Bone() { Name = "Right Thumb Intermediate", Transform = null },
                    new Bone() { Name = "Right Thumb Proximal", Transform = null },
                },
            },
            new BoneGroup() {
                GroupName = "Right Arm", 
                Bones = new Bone[]
                {
                    new Bone() { Name = "RightShoulder", Transform = null },
                    new Bone() { Name = "RightUpperArm", Transform = null },
                    new Bone() { Name = "RightLowerArm", Transform = null },
                    new Bone() { Name = "RightHand", Transform = null },
                },
            },
            new BoneGroup() {
                GroupName = "Spine", 
                Bones = new Bone[]
                {
                    new Bone() { Name = "Hips", Transform = null },
                    new Bone() { Name = "Spine", Transform = null },
                    new Bone() { Name = "Chest", Transform = null },
                    new Bone() { Name = "UpperChest", Transform = null },
                    new Bone() { Name = "Neck", Transform = null },
                    new Bone() { Name = "Head", Transform = null },
                },
            },
        };
        
        private SkeletonBuilder skeletonBuilder = new SkeletonBuilder();

        [MenuItem("Tools/Skeleton Builder")]
        public static void Generate()
        {
            var window = GetWindow<SkeletonBuilderEditor>("Skeleton Builder");
            window.minSize = new Vector2(700, 120);
        }

        private void OnGUI()
        {
            avatar = EditorGUILayout.ObjectField("Avatar", avatar, typeof(Transform), true) as Transform;
            skeletonRoot = EditorGUILayout.ObjectField("Root", skeletonRoot, typeof(Transform), true) as Transform;
            
            for (int i = 0; i < BoneGroups.Length; i++)
            {
                var boneGroup = BoneGroups[i];
                boneGroup.Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(boneGroup.Foldout, boneGroup.GroupName);
                if (boneGroup.Foldout)
                {
                    EditorGUI.indentLevel++;
                    
                    foreach (var bone in boneGroup.Bones)
                    {
                        bone.Transform = EditorGUILayout.ObjectField(bone.Name, bone.Transform, typeof(Transform), true) as Transform;
                    }
                    
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            
            if (GUILayout.Button("Build and Save Avatar Skeleton Definition"))
            {
                skeletonBuilder.Build(BoneGroups, avatar, skeletonRoot);
            }
            
            GUILayout.Space(20);
            
            targetAvatar = EditorGUILayout.ObjectField("Target Avatar", targetAvatar, typeof(Transform), true) as Transform;
            definition = EditorGUILayout.ObjectField("Definition", definition, typeof(AvatarSkeletonDefinition), true) as AvatarSkeletonDefinition;
            
            if (GUILayout.Button("Load from Avatar Skeleton Definition"))
            {
                skeletonRoot = targetAvatar.Find(definition.rootBonePath);
                skeletonBuilder.Load(definition, BoneGroups, ref skeletonRoot);
            }
        }
    }
}

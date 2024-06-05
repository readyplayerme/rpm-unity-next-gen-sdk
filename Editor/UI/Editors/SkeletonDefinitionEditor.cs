using System.Collections.Generic;
using ReadyPlayerMe.Data;
using UnityEditor;

namespace ReadyPlayerMe.Editor.UI.Editors
{
    [CustomEditor(typeof(SkeletonDefinition))]
    public class SkeletonDefinitionEditor : UnityEditor.Editor
    {
        private SerializedProperty boneGroupsProp;

        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        private void OnEnable()
        {
            boneGroupsProp = serializedObject.FindProperty("BoneGroups");

            // Initialize foldout states
            for (int i = 0; i < boneGroupsProp.arraySize; i++)
            {
                SerializedProperty boneGroupProp = boneGroupsProp.GetArrayElementAtIndex(i);
                SerializedProperty groupNameProp = boneGroupProp.FindPropertyRelative("GroupName");
                foldoutStates[groupNameProp.stringValue] = false;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // a field for Root
            SerializedProperty rootProp = serializedObject.FindProperty("Root");
            EditorGUILayout.PropertyField(rootProp);
            
            for (int i = 0; i < boneGroupsProp.arraySize; i++)
            {
                SerializedProperty boneGroupProp = boneGroupsProp.GetArrayElementAtIndex(i);
                SerializedProperty groupNameProp = boneGroupProp.FindPropertyRelative("GroupName");
                SerializedProperty bonesKeysProp = boneGroupProp.FindPropertyRelative("BonesKeys");
                SerializedProperty bonesValuesProp = boneGroupProp.FindPropertyRelative("BonesValues");
            
                // Display foldout for each bone group
                foldoutStates[groupNameProp.stringValue] = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutStates[groupNameProp.stringValue], groupNameProp.stringValue);
            

                if (foldoutStates[groupNameProp.stringValue])
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUI.indentLevel++;

                    // Display each bone in the dictionary
                    for (int j = 0; j < bonesKeysProp.arraySize; j++)
                    {
                        SerializedProperty keyProp = bonesKeysProp.GetArrayElementAtIndex(j);
                        SerializedProperty valueProp = bonesValuesProp.GetArrayElementAtIndex(j);

                        EditorGUI.BeginChangeCheck();
                        string key = keyProp.stringValue;
                        string value = valueProp.stringValue;
                        string newValue = EditorGUILayout.TextField(key, value);
                        if (EditorGUI.EndChangeCheck())
                        {
                            valueProp.stringValue = newValue;
                        }
                    }

                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
            
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

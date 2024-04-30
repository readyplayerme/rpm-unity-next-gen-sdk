using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.Tools.Editor.UI.Components
{
    public class ObjectInput
    {
        private Object Value { get; set; }

        public void Init(string objectId)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(objectId);
            
            Value = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        }

        public void Render(Action<Object> onChange)
        {
            var newValue = EditorGUILayout.ObjectField(Value, typeof(GameObject), false);

            if (newValue == Value)
                return;

            Value = newValue;
            onChange(Value);
        }
    }
}
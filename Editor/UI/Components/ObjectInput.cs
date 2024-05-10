using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.Editor.UI.Components
{
    public class ObjectInput<T> where T : Object
    {
        private T Value { get; set; }

        public void Init(string objectId)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(objectId);
            
            Value = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public void Render(Action<T> onChange)
        {
            var newValue = EditorGUILayout.ObjectField(Value, typeof(T), false) as T;

            if (newValue == Value)
                return;

            Value = newValue;
            onChange(Value);
        }
    }
}
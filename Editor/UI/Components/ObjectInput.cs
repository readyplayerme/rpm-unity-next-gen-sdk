using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PlayerZero.Editor.UI.Components
{
    public class ObjectInput<T> where T : Object
    {
        private T Value { get; set; }

        public void Init(string objectId)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(objectId);

            Value = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public void Render(Action<T> onChange, string label = null)
        {
            var newValue = string.IsNullOrEmpty(label)
                ? EditorGUILayout.ObjectField(Value, typeof(T), false) as T
                : EditorGUILayout.ObjectField(label, Value, typeof(T), false) as T;

            if (newValue == Value)
                return;

            Value = newValue;
            onChange(Value);
        }
    }
}
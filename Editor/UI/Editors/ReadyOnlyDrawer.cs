using UnityEditor;
using UnityEngine;

namespace PlayerZero.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable editing
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; // Re-enable editing for other fields
        }
    }
}
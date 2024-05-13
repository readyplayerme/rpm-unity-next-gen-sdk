using System;
using UnityEditor;

namespace ReadyPlayerMe.Editor.UI.Components
{
    public class TextInput
    {
        private string _currentValue;

        public void Init(string defaultValue)
        {
            _currentValue = defaultValue;
        }

        public void Render(string label, Action<string> onChange)
        {
            var newSelection = EditorGUILayout.TextField(label, _currentValue);

            if (newSelection == _currentValue)
                return;
            
            _currentValue = newSelection;
            
            onChange(_currentValue);
        }
    }
}
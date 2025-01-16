using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PlayerZero.Editor.UI.Components
{
    public class Option
    {
        public string Label { get; set; }
        
        public string Value { get; set; }
    }
    
    public class SelectInput
    {
        private IList<Option> _options;

        private int _selectedIndex = 0;

        public void Init(IList<Option> options, string defaultValue)
        {
            _options = options;
            
            var defaultOption = options.FirstOrDefault(p => p.Value == defaultValue);
            _selectedIndex = options.IndexOf(defaultOption);
        }

        public void Render(Action<string> onChange)
        {
            if (_options == null)
                return;

            var newSelection = EditorGUILayout.Popup(_selectedIndex, _options
                .Select(p => p.Label)
                .ToArray());

            if (newSelection == _selectedIndex)
                return;
            
            _selectedIndex = newSelection;
            
            onChange(_options[_selectedIndex].Value);
        }
    }
}
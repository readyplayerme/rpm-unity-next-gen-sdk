using System.Linq;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.UI.Components;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CharacterTemplateView
    {
        private readonly CharacterTemplateViewModel _viewModel;
        private readonly ObjectInput<GameObject> _templateInput;
        private readonly TextInput _tagInput;

        public CharacterTemplateView(CharacterTemplateViewModel viewModel)
        {
            _viewModel = viewModel;
            _templateInput = new ObjectInput<GameObject>();
            _tagInput = new TextInput();
        }

        public void Init(CharacterStyleTemplate template)
        {
            _viewModel.Init(template);
            _templateInput.Init(_viewModel.CharacterStyleTemplate.cacheId);
            _tagInput.Init(_viewModel.CharacterStyleTemplate.tags.FirstOrDefault());
        }

        public void Render()
        {
            _viewModel.IsOpen = EditorGUILayout.Foldout(_viewModel.IsOpen, "ID: " + _viewModel.CharacterStyleTemplate.id);

            if (!_viewModel.IsOpen)
                return;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    _templateInput.Render(onChange: o => { _viewModel.SaveTemplate(o); }, "Template");
                    _tagInput.Render("Tag", onChange: o => { _viewModel.SaveTag(o); });

                    GUILayout.Space(2);
                    
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Delete"))
                    {
                        var templateConfig = Resources.Load<CharacterStyleTemplateConfig>("CharacterStyleTemplateConfig");
                        templateConfig.templates = templateConfig.templates
                            .Where(p => p.id != _viewModel.CharacterStyleTemplate.id)
                            .ToArray();
                        
                        EditorUtility.SetDirty(templateConfig);
                        AssetDatabase.Refresh();
                    }
                    
                    GUILayout.Space(8);

                    GUI.backgroundColor = Color.white;
                }
            }
        }
    }
}
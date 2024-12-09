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

        public void Init(CharacterBlueprintTemplate template)
        {
            _viewModel.Init(template);
            _templateInput.Init(_viewModel.CharacterBlueprintTemplate.cacheId);
            _tagInput.Init(_viewModel.CharacterBlueprintTemplate.tags.FirstOrDefault());
        }

        public void Render()
        {
            var hasTag = _viewModel.CharacterBlueprintTemplate.tags.Count > 0 &&
                         !string.IsNullOrEmpty(_viewModel.CharacterBlueprintTemplate.tags[0]);

            _viewModel.IsOpen = EditorGUILayout.Foldout(_viewModel.IsOpen,
                hasTag
                    ? _viewModel.CharacterBlueprintTemplate.tags[0]
                    : _viewModel.CharacterBlueprintTemplate.id
            );

            if (!_viewModel.IsOpen)
                return;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label($"ID: {_viewModel.CharacterBlueprintTemplate.id}");
                    _templateInput.Render(onChange: o => { _viewModel.SaveTemplate(o); }, "Template");
                    _tagInput.Render("Tag", onChange: o => { _viewModel.SaveTag(o); });

                    GUILayout.Space(2);

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Delete"))
                    {
                        var templateConfig =
                            Resources.Load<CharacterBlueprintTemplateConfig>("CharacterBlueprintTemplateConfig");
                        
                        templateConfig.templates = templateConfig.templates
                            .Where(p => p.id != _viewModel.CharacterBlueprintTemplate.id)
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
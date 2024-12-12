using ReadyPlayerMe.Data;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Editor.UI.Components;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CharacterBlueprintView
    {
        private readonly CharacterBlueprintViewModel _viewModel;
        private readonly ObjectInput<SkeletonDefinition> _boneDefinitionInput;
        private readonly ObjectInput<CharacterTemplate> _defaultBlueprintInput;

        public CharacterBlueprintView(CharacterBlueprintViewModel viewModel)
        {
            _viewModel = viewModel;
            _boneDefinitionInput = new ObjectInput<SkeletonDefinition>();
            _defaultBlueprintInput = new ObjectInput<CharacterTemplate>();
        }

        public async Task Init(CharacterBlueprint characterBlueprint)
        {
            await _viewModel.Init(characterBlueprint);
            
            _boneDefinitionInput.Init(_viewModel.BoneDefinitionCacheId);
        }

        public void Render()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(new GUIStyle()
                       {
                           fixedWidth = 120
                       }))
                {
                    using (new EditorGUILayout.HorizontalScope(new GUIStyle()
                           {
                               normal = new GUIStyleState()
                               {
                                   background = Texture2D.grayTexture,
                               },
                               margin = new RectOffset(5, 5, 5, 5),
                           }))
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(_viewModel.Image,
                            new GUIStyle()
                            {
                                stretchWidth = true,
                                stretchHeight = true,
                                fixedWidth = 90,
                                fixedHeight = 90,
                                alignment = TextAnchor.MiddleCenter,
                            });
                        GUILayout.FlexibleSpace();
                    }

                    if (GUILayout.Button("Load Blueprint"))
                    {
#pragma warning disable CS4014
                        _viewModel.LoadBlueprintAsync();
#pragma warning restore CS4014
                    }
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("ID: " + _viewModel.CharacterBlueprint.Id, EditorStyles.label);
                    GUILayout.Space(3); 
                    EditorGUILayout.LabelField("Skeleton Definition", EditorStyles.boldLabel);
                    _boneDefinitionInput.Render(onChange: o => { _viewModel.SaveBoneDefinition(o); });
                    EditorGUILayout.LabelField("Character Template", EditorStyles.boldLabel);
                    _defaultBlueprintInput.Render(onChange: x => { _viewModel.SaveCharacterBlueprintTemplate(x); });
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}
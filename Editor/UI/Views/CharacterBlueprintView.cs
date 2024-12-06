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

        public CharacterBlueprintView(CharacterBlueprintViewModel viewModel)
        {
            _viewModel = viewModel;
            _boneDefinitionInput = new ObjectInput<SkeletonDefinition>();
        }

        public async Task Init(Asset characterBlueprint)
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
                    GUILayout.Space(8);
                    EditorGUILayout.SelectableLabel("ID: " + _viewModel.CharacterBlueprint.Id);
                    GUILayout.Label("Skeleton Definition");
                    _boneDefinitionInput.Render(onChange: o => { _viewModel.SaveBoneDefinition(o); });
                }
            }
        }
    }
}
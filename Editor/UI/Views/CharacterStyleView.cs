﻿using ReadyPlayerMe.Data;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Editor.UI.Components;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CharacterStyleView
    {
        private readonly CharacterStyleViewModel _viewModel;
        private readonly ObjectInput<GameObject> _templateInput;
        private readonly ObjectInput<AvatarSkeletonDefinition> _avatarBoneDefinitionInput;

        public CharacterStyleView(CharacterStyleViewModel viewModel)
        {
            _viewModel = viewModel;
            _templateInput = new ObjectInput<GameObject>();
            _avatarBoneDefinitionInput = new ObjectInput<AvatarSkeletonDefinition>();
        }

        public async Task Init(Asset characterStyle)
        {
            await _viewModel.Init(characterStyle);

            _templateInput.Init(_viewModel.TemplateCacheId);
            _avatarBoneDefinitionInput.Init(_viewModel.AvatarBoneDefinitionCacheId);
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

                    if (GUILayout.Button("Load Style"))
                    {
                        Debug.Log("here 2");
                        
#pragma warning disable CS4014
                        _viewModel.LoadStyleAsync();
#pragma warning restore CS4014
                    }
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(8);
                    GUILayout.Label("ID: " + _viewModel.CharacterStyle.Id);
                    GUILayout.Label("Style Template Prefab");
                    _templateInput.Render(onChange: o => { _viewModel.SaveTemplate(o); });
                    GUILayout.Label("Test Avatar");
                    _avatarBoneDefinitionInput.Render(onChange: o => { _viewModel.SaveAvatarBoneDefinition(o); });
                }
            }
        }
    }
}
using System.Threading.Tasks;
using ReadyPlayerMe.Runtime.Data.V1;
using ReadyPlayerMe.Tools.Editor.UI.Components;
using ReadyPlayerMe.Tools.Editor.UI.ViewModels;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.Views
{
    public class CharacterStyleView
    {
        private readonly CharacterStyleViewModel _viewModel;
        private readonly ObjectInput _templateInput;

        public CharacterStyleView(CharacterStyleViewModel viewModel)
        {
            _viewModel = viewModel;
            _templateInput = new ObjectInput();
        }

        public async Task Init(Asset characterStyle)
        {
            await _viewModel.Init(characterStyle);

            _templateInput.Init(_viewModel.CacheId);
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
                        _viewModel.LoadStyleAsync();
                    }
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(8);
                    GUILayout.Label("ID: " + _viewModel.CharacterStyle.Id);
                    GUILayout.Label("Template");
                    _templateInput.Render(onChange: o => { _viewModel.SaveTemplate(o); });
                }
            }
        }
    }
}
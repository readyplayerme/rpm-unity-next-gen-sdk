using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CharacterStylesView
    {
        private readonly CharacterStylesViewModel _viewModel;
        private IList<CharacterStyleView> _characterStyleViews;

        public CharacterStylesView(CharacterStylesViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task InitAsync()
        {
            await _viewModel.Init();

            _characterStyleViews = await Task.WhenAll(_viewModel.CharacterStyles.Select(async style =>
            {
                var viewModel = new CharacterStyleViewModel();
                var view = new CharacterStyleView(viewModel);
                await view.Init(style);
                return view;
            }));
        }

        public void Render()
        {
            if (_viewModel.Loading)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Character styles loading...", new GUIStyle()
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = new GUIStyleState() 
                        {
                            textColor = Color.white
                        }
                    });
                    GUILayout.FlexibleSpace();
                }

                return;
            }

            GUILayout.Label("Character Styles", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                margin = new RectOffset(10, 10, 0, 0)
            });

            if (!_viewModel.Loading && _viewModel.CharacterStyles?.Count is null or 0)
            {
                GUILayout.Label("You have no character styles setup for this application.",
                    new GUIStyle(GUI.skin.label)
                    {
                        margin = new RectOffset(9, 10, 0, 0)
                    });

                if (GUILayout.Button("Create first style in Studio", new GUIStyle(GUI.skin.button)
                    {
                        normal =
                        {
                            textColor = Color.white
                        },
                        margin = new RectOffset(12, 12, 5, 0),
                        alignment = TextAnchor.MiddleCenter,
                    }))
                {
                    Application.OpenURL("https://studio.readyplayer.me/");
                }

                return;
            }

            GUILayout.Label("Here you can configure the in-game templates for each of your character styles",
                new GUIStyle(GUI.skin.label)
                {
                    margin = new RectOffset(9, 10, 0, 0)
                });

            using (new GUILayout.VerticalScope(new GUIStyle
                   {
                       margin = new RectOffset(9, 9, 5, 5)
                   }))
            {
                if (_characterStyleViews == null)
                    return;
                
                foreach (var characterStyleView in _characterStyleViews)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        characterStyleView.Render();
                    }
                }
            }
        }
    }
}
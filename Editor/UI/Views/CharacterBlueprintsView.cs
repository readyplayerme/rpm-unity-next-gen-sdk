using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Editor.UI.ViewModels;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace ReadyPlayerMe.Editor.UI.Views
{
    public class CharacterBlueprintsView
    {
        private readonly CharacterBlueprintsViewModel _viewModel;
        private IList<CharacterBlueprintView> _characterBlueprintViews;

        public CharacterBlueprintsView(CharacterBlueprintsViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task InitAsync()
        {
            await _viewModel.Init();

            _characterBlueprintViews = await Task.WhenAll(_viewModel.CharacterBlueprints.Select(async style =>
            {
                var viewModel = new CharacterBlueprintViewModel(_viewModel.AnalyticsApi);
                var view = new CharacterBlueprintView(viewModel);
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
                    GUILayout.Label("Character blueprints loading...", new GUIStyle()
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

            GUILayout.Label("Character Blueprints", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                margin = new RectOffset(10, 10, 0, 0)
            });

            if (!_viewModel.Loading && _viewModel.CharacterBlueprints?.Count is null or 0)
            {
                GUILayout.Label("You have no character blueprints setup for this application.",
                    new GUIStyle(GUI.skin.label)
                    {
                        margin = new RectOffset(9, 10, 0, 0)
                    });

                if (GUILayout.Button("Create first blueprint in Studio", new GUIStyle(GUI.skin.button)
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

            GUILayout.Label("Here you can import your character blueprints from Studio.",
                new GUIStyle(GUI.skin.label)
                {
                    margin = new RectOffset(9, 10, 0, 0)
                });

            using (new GUILayout.VerticalScope(new GUIStyle
                   {
                       margin = new RectOffset(9, 9, 5, 5)
                   }))
            {
                if (_characterBlueprintViews == null)
                    return;
                
                foreach (var characterBlueprintView in _characterBlueprintViews)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        characterBlueprintView.Render();
                    }
                }
            }
        }
    }
}
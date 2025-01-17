using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayerZero.Data;
using PlayerZero.Editor.UI.ViewModels;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace PlayerZero.Editor.UI.Views
{
    public class CharacterBlueprintsView
    {
        private readonly CharacterBlueprintListViewModel blueprintListViewModel;
        private IList<CharacterBlueprintView> _characterBlueprintViews;

        public CharacterBlueprintsView(CharacterBlueprintListViewModel blueprintListViewModel)
        {
            this.blueprintListViewModel = blueprintListViewModel;
        }

        public async Task InitAsync()
        {
            await blueprintListViewModel.Init();
            var applicationId = Resources.Load<Settings>( "ReadyPlayerMeSettings" )?.ApplicationId;
            var characterTemplateList = Resources.Load<CharacterTemplateConfig>(applicationId);
  
            _characterBlueprintViews = await Task.WhenAll(blueprintListViewModel.CharacterBlueprints.Select(async blueprint =>
            {
                var blueprintViewModel = new CharacterBlueprintViewModel(blueprintListViewModel.AnalyticsApi);
                var blueprintView = new CharacterBlueprintView(blueprintViewModel);
                await blueprintView.Init(blueprint, characterTemplateList);
                return blueprintView;
            }));
        }

        public void Render()
        {
            if (blueprintListViewModel.Loading)
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

            if (!blueprintListViewModel.Loading && blueprintListViewModel.CharacterBlueprints?.Count is null or 0)
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
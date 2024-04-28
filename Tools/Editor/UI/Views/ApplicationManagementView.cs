using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Tools.Editor.UI.Components;
using ReadyPlayerMe.Tools.Editor.UI.ViewModels;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.Views
{
    public class ApplicationManagementView
    {
        private readonly ApplicationManagementViewModel _viewModel;
        private readonly SelectInput _selectInput;
        private readonly CharacterStylesView _characterStylesView;
        
        public ApplicationManagementView(ApplicationManagementViewModel viewModel)
        {
            _viewModel = viewModel;
            _selectInput = new SelectInput();

            var characterStylesViewModel = new CharacterStylesViewModel(viewModel.AssetApi, viewModel.Settings);
            _characterStylesView = new CharacterStylesView(characterStylesViewModel);
        }

        public async Task Init()
        {
            await _viewModel.Init();
            
            _selectInput.Init(
                _viewModel.Applications
                    .ToList()
                    .Select(app => new Option()
                    {
                        Label = $"{app.Name}",
                        Value = app.Id,
                    })
                    .ToArray(),
                _viewModel.Settings.ApplicationId
            );

            await _characterStylesView.InitAsync();
        }

        public void Render()
        {
            GUILayout.Space(15);

            if (_viewModel.Loading)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Loading...", new GUIStyle()
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

            GUILayout.Label("Project Settings", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                margin = new RectOffset(10, 10, 0, 0),
                fontSize = 14
            });

            GUILayout.Label("Select the Ready Player Me application to link to project", new GUIStyle(GUI.skin.label)
            {
                margin = new RectOffset(9, 10, 0, 0)
            });

            using (new GUILayout.HorizontalScope(new GUIStyle()
                   {
                       margin = new RectOffset(7, 7, 5, 0)
                   }))
            {
                _selectInput.Render(async (applicationId) =>
                {
                    _viewModel.Settings.ApplicationId = applicationId;

                    await _characterStylesView.InitAsync();
                });
            }


            GUILayout.Label(_viewModel.Error, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    textColor = Color.red
                },
                margin = new RectOffset()
                {
                    left = 10
                }
            });

            GUILayout.Space(10);

            _characterStylesView.Render();
        }
    }
}
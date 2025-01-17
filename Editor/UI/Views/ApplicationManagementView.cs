using System.Linq;
using UnityEditor;
using UnityEngine;
using PlayerZero.Data;
using System.Threading.Tasks;
using PlayerZero.Editor.UI.Components;
using PlayerZero.Editor.UI.ViewModels;

namespace PlayerZero.Editor.UI.Views
{
    public class ApplicationManagementView
    {
        private readonly ApplicationManagementViewModel _viewModel;
        private readonly SelectInput _selectInput;
        private readonly TextInput _textInput;
        private readonly CharacterBlueprintsView characterBlueprintsView;
        private Vector2 _scrollPosition = Vector2.zero;
        private string applicationId;
        
        public ApplicationManagementView(ApplicationManagementViewModel viewModel)
        {
            _viewModel = viewModel;
            _selectInput = new SelectInput();
            _textInput = new TextInput();

            var characterBlueprintsViewModel = new CharacterBlueprintListViewModel(viewModel.BlueprintApi, viewModel.Settings, _viewModel.AnalyticsApi);
            characterBlueprintsView = new CharacterBlueprintsView(characterBlueprintsViewModel);
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
            _textInput.Init(_viewModel.Settings.ApiKey);
            applicationId = _viewModel.Settings.ApplicationId;
            await CharacterTemplateConfigCreator.LoadAndCreateTemplateList(applicationId);
            await characterBlueprintsView.InitAsync();
        }

        public void Render()
        {
            using var scrollViewScope = new GUILayout.ScrollViewScope(_scrollPosition, false, false);
            _scrollPosition = scrollViewScope.scrollPosition;

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

            GUILayout.Label("Select the Player Zero application to link to project",
                new GUIStyle(GUI.skin.label)
                {
                    margin = new RectOffset(9, 10, 0, 0)
                });
            EditorGUILayout.EndFoldoutHeaderGroup();

            using (new GUILayout.HorizontalScope(new GUIStyle()
                   {
                       margin = new RectOffset(7, 7, 5, 0)
                   }))
            {
                _selectInput.Render(async (applicationId) =>
                {
                    _viewModel.Settings.ApplicationId = applicationId;
                    EditorUtility.SetDirty(_viewModel.Settings);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    await characterBlueprintsView.InitAsync();
                });
            }

            if (!string.IsNullOrEmpty(_viewModel.Error))
            {
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
            }

            GUILayout.Space(20);

            characterBlueprintsView.Render();

            GUILayout.Space(20);
            
            GUILayout.Label("Auth Settings", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                margin = new RectOffset(10, 10, 0, 0),
                fontSize = 14
            });

            using (new GUILayout.VerticalScope(new GUIStyle()
                   {
                       margin = new RectOffset(7, 7, 5, 0)
                   }))
            {
                _viewModel.Settings.ApiProxyUrl =
                    EditorGUILayout.TextField("Proxy Api Url", _viewModel.Settings.ApiProxyUrl);

                GUILayout.Space(5);

                _textInput.Render("Api Key", (value) =>
                {
                    _viewModel.Settings.ApiKey = value;
                    EditorUtility.SetDirty(_viewModel.Settings);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                });

                GUILayout.Space(5);

                EditorGUILayout.HelpBox(
                    "Setting your API Key in the field above can be insecure as it means that your API Key can be discoverable in your game build, it is advisable to instead setup a proxy server. See our docs for more details.",
                    MessageType.Info
                );
            }

            GUILayout.Space(20);
        }
    }
}
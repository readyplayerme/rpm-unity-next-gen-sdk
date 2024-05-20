using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Auth;
using ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Editor.Cache;
using ReadyPlayerMe.Editor.EditorPrefs;
using ReadyPlayerMe.Editor.UI.ViewModels;
using ReadyPlayerMe.Editor.UI.Views;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.Windows
{
    public class ReadyPlayerMeEditor : EditorWindow
    {
        private DeveloperLoginView _developerLoginView;
        private ApplicationManagementView _applicationManagementView;

        [MenuItem("Tools/Ready Player Me/Project Settings")]
        public static void Generate()
        {
            var window = GetWindow<ReadyPlayerMeEditor>("Ready Player Me");
            window.minSize = new Vector2(700, 120);
        }

        private async void OnEnable()
        {
            var developerAuthApi = new DeveloperAuthApi();
            var developerAccountApi = new DeveloperAccountApi();
            var assetApi = new AssetApi();
            assetApi.SetAuthenticationStrategy(new DeveloperTokenAuthStrategy());

            var settingsCache = new ScriptableObjectCache<Settings>();
            var settings = settingsCache.Init("ReadyPlayerMeSettings");

            var templateCache = new ScriptableObjectCache<CharacterStyleTemplateConfig>();
            templateCache.Init("CharacterStyleTemplateConfig");

            var developerLoginViewModel = new DeveloperLoginViewModel(developerAuthApi);
            _developerLoginView = new DeveloperLoginView(developerLoginViewModel);

            var projectDetailsViewModel = new ApplicationManagementViewModel(
                assetApi,
                developerAccountApi,
                settings
            );

            _applicationManagementView = new ApplicationManagementView(projectDetailsViewModel);
            if (DeveloperAuthCache.Exists())
                await _applicationManagementView.Init();
        }

        private void OnGUI()
        {
            if (!DeveloperAuthCache.Exists())
            {
                _developerLoginView.Render(async () => { await _applicationManagementView.Init(); });
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"Welcome {DeveloperAuthCache.Data.Name},", new GUIStyle()
                {
                    normal = new GUIStyleState()
                    {
                        textColor = Color.white
                    },
                    margin = new RectOffset(5, 5, 7, 5),
                });

                if (GUILayout.Button("Sign Out", new GUIStyle()
                    {
                        fixedWidth = 70,
                        normal = new GUIStyleState()
                        {
                            background = Texture2D.grayTexture,
                            textColor = Color.white
                        },
                        margin = new RectOffset(5, 5, 5, 5),
                        alignment = TextAnchor.MiddleCenter
                    }))
                {
                    DeveloperAuthCache.Delete();
                }
            }

            _applicationManagementView.Render();
        }
    }
}
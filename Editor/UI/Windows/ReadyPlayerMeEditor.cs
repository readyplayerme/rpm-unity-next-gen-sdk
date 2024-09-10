using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using ReadyPlayerMe.Editor.Api.V1.Auth;
using ReadyPlayerMe.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Editor.Cache;
using ReadyPlayerMe.Editor.Cache.EditorPrefs;
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

        [MenuItem("Tools/Ready Player Me/Style Manager", false, 0)]
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
            var analyticsApi = new AnalyticsApi();
            assetApi.SetAuthenticationStrategy(new DeveloperTokenAuthStrategy());

            var settingsCache = new ScriptableObjectCache<Settings>();
            var settings = settingsCache.Init("ReadyPlayerMeSettings");

            var templateCache = new ScriptableObjectCache<CharacterStyleTemplateConfig>();
            templateCache.Init("CharacterStyleTemplateConfig");
            
            var skeletonDefinitionCache = new ScriptableObjectCache<SkeletonDefinitionConfig>();
            skeletonDefinitionCache.Init("SkeletonDefinitionConfig");

            var developerLoginViewModel = new DeveloperLoginViewModel(developerAuthApi, analyticsApi);
            _developerLoginView = new DeveloperLoginView(developerLoginViewModel);

            var projectDetailsViewModel = new ApplicationManagementViewModel(
                analyticsApi,
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
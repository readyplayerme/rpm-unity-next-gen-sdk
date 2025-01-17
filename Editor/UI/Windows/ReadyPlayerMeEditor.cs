using PlayerZero.Api.V1;
using PlayerZero.Data;
using PlayerZero.Editor.Api.V1.Analytics;
using PlayerZero.Editor.Api.V1.Auth;
using PlayerZero.Editor.Api.V1.DeveloperAccounts;
using PlayerZero.Editor.Cache;
using PlayerZero.Editor.Cache.EditorPrefs;
using PlayerZero.Editor.UI.ViewModels;
using PlayerZero.Editor.UI.Views;
using UnityEditor;
using UnityEngine;

namespace PlayerZero.Editor.UI.Windows
{
    public class ReadyPlayerMeEditor : EditorWindow
    {
        private DeveloperLoginView _developerLoginView;
        private ApplicationManagementView _applicationManagementView;
        
        [MenuItem("Tools/Player Zero", false, 0)]
        public static void Generate()
        {
            var window = GetWindow<ReadyPlayerMeEditor>("Player Zero");
            window.minSize = new Vector2(700, 120);
        }

        private async void OnEnable()
        {
            var developerAuthApi = new DeveloperAuthApi();
            var developerAccountApi = new DeveloperAccountApi();
            var blueprintApi = new BlueprintApi();
            var analyticsApi = new AnalyticsApi();

            var settingsCache = new ScriptableObjectCache<Settings>();
            var settings = settingsCache.Init("ReadyPlayerMeSettings");
            
            var skeletonDefinitionCache = new ScriptableObjectCache<SkeletonDefinitionConfig>();
            skeletonDefinitionCache.Init("SkeletonDefinitionConfig");

            var developerLoginViewModel = new DeveloperLoginViewModel(developerAuthApi, analyticsApi);
            _developerLoginView = new DeveloperLoginView(developerLoginViewModel);

            var projectDetailsViewModel = new ApplicationManagementViewModel(
                analyticsApi,
                blueprintApi,
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
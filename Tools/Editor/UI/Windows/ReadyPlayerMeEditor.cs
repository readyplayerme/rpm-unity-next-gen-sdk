using ReadyPlayerMe.Tools.Editor.Api.V1.DeveloperAccounts;
using ReadyPlayerMe.Tools.Editor.Cache;
using ReadyPlayerMe.Tools.Editor.UI.Components;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Tools.Editor.UI.Windows
{
    public class ReadyPlayerMeEditor : EditorWindow
    {
        private DeveloperAccountApi _developerAccountApi;

        private DeveloperLoginPage _developerLoginPage;
        private ProjectDetailsPage _projectDetailsPage;

        [MenuItem("Tools/Ready Player Me")]
        public static void Generate()
        {
            var window = GetWindow<ReadyPlayerMeEditor>("Ready Player Me");
            window.minSize = new Vector2(700, 120);
        }

        private async void OnEnable()
        {
            _developerAccountApi = new DeveloperAccountApi();

            _developerLoginPage = new DeveloperLoginPage(_developerAccountApi);
            _projectDetailsPage = new ProjectDetailsPage(_developerAccountApi);

            if (DeveloperDetailsCache.Exists())
                await _projectDetailsPage.Init();
        }

        private async void OnGUI()
        {
            if (!DeveloperDetailsCache.Exists())
            {
                await _developerLoginPage.Render(async () => { await _projectDetailsPage.Init(); });
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"Welcome {DeveloperDetailsCache.Data.Name},", new GUIStyle()
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
                    DeveloperDetailsCache.Delete();
                }
            }

            await _projectDetailsPage.Render();
        }
    }
}
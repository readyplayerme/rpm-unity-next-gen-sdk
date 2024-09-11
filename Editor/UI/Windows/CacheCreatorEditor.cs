using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Editor.UI.Views;
using ReadyPlayerMe.Editor.UI.ViewModels;

namespace ReadyPlayerMe.Editor.UI.Windows
{
    public class CacheCreatorEditor : EditorWindow
    {
        private CacheCreatorView cacheCreatorView;

        [MenuItem("Tools/Ready Player Me/Cache Creator", false, 1)]
        public static void Generate()
        {
            var window = GetWindow<CacheCreatorEditor>("Ready Player Me Cache Creator");
            window.minSize = new Vector2(500, 120);
        }

        private void OnEnable()
        {
            var viewModel = new CacheCreatorViewModel();
            cacheCreatorView = new CacheCreatorView(viewModel);
        }

        private void OnGUI()
        {
            cacheCreatorView.Render();
        }
    }
}
using System.IO;
using UnityEngine;
using AssetDatabase = UnityEditor.AssetDatabase;
using TextAsset = UnityEngine.TextCore.Text.TextAsset;

namespace ReadyPlayerMe.Editor.Cache
{
    public abstract class Cache
    {
        private const string BaseDirectory = "Assets/Ready Player Me/Resources/";

        private readonly string _name;

        protected string CacheDirectory => BaseDirectory + _name;

        protected Cache(string name)
        {
            _name = name;

            EnsureFoldersExist();
        }

        private void EnsureFoldersExist()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me"))
                AssetDatabase.CreateFolder("Assets", "Ready Player Me");

            if (!AssetDatabase.IsValidFolder("Assets/Ready Player Me/Resources"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Resources");

            if (AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Ready Player Me/Resources/README.txt") == null)
                CreateTextAsset("README.txt",
                    "This folder is managed by the Ready Player Me SDK, and you should not make manual changes.",
                    "Assets/Ready Player Me/Resources"
                );

            if (!AssetDatabase.IsValidFolder($"Assets/Ready Player Me/Resources/{_name}"))
                AssetDatabase.CreateFolder("Assets/Ready Player Me/Resources", _name);

            AssetDatabase.Refresh();
        }

        private static void CreateTextAsset(string name, string content, string directory)
        {
            File.WriteAllText($"{directory}/{name}", content);
        }

        public static string FindAssetGuid(Object asset)
        {
            if (asset == null)
                return string.Empty;

            var assetPath = AssetDatabase.GetAssetPath(asset);

            return string.IsNullOrEmpty(assetPath)
                ? string.Empty
                : AssetDatabase.AssetPathToGUID(assetPath);
        }
    }
}
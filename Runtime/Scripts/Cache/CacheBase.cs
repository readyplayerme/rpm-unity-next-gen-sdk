using System.IO;
using UnityEngine.TextCore.Text;
using AssetDatabase = UnityEditor.AssetDatabase;

namespace ReadyPlayerMe.Cache
{
    public abstract class CacheBase
    {
        private const string BaseDirectory = "Assets/Ready Player Me/Resources/";
        private readonly string _name;

        protected string CacheDirectory => BaseDirectory + _name;

        protected CacheBase(string name)
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
    }
}
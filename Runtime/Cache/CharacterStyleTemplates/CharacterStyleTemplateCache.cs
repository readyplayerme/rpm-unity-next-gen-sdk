using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Runtime.Cache
{
    public static class CharacterStyleTemplateCache
    {
        private const string CharacterTemplatesDirectory = "Assets/Ready Player Me/Character Templates";

        public static void Save(Object template, string id)
        {
            EnsureFoldersExist();

            AssetDatabase.DeleteAsset($"{CharacterTemplatesDirectory}/{id}.prefab");

            if (template == null)
                return;

            var pathToExistingAsset = FindPathToExistingAsset(template);

            if (!string.IsNullOrEmpty(pathToExistingAsset))
                AssetDatabase.CopyAsset(pathToExistingAsset, $"{CharacterTemplatesDirectory}/{id}.prefab");
            else
                AssetDatabase.CreateAsset(template, $"{CharacterTemplatesDirectory}/{id}.prefab");

            AssetDatabase.SaveAssets();
        }

        public static GameObject Load(string characterStyleId)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(
                $"{CharacterTemplatesDirectory}/{characterStyleId}.prefab");
        }

        public static string GetCacheId(string characterStyleId)
        {
            return AssetDatabase
                .FindAssets(characterStyleId, new string[] { CharacterTemplatesDirectory })
                .FirstOrDefault();
        }

        private static void EnsureFoldersExist()
        {
            if (!AssetDatabase.IsValidFolder($"Assets/Ready Player Me"))
                AssetDatabase.CreateFolder("Assets", "Ready Player Me");

            if (!AssetDatabase.IsValidFolder(CharacterTemplatesDirectory))
                AssetDatabase.CreateFolder("Assets/Ready Player Me", "Character Templates");
        }

        private static string FindPathToExistingAsset(Object template)
        {
            var guids = new NativeArray<GUID>(new GUID[1]
                {
                    GUID.Generate(),
                },
                Allocator.Temp
            );

            AssetDatabase.InstanceIDsToGUIDs(
                new NativeArray<int>(new int[] { template.GetInstanceID() }, Allocator.Temp),
                guids
            );

            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }
    }
}
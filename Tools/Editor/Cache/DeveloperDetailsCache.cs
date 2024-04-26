using Newtonsoft.Json;
using ReadyPlayerMe.Tools.Editor.Data;
using UnityEditor;

namespace ReadyPlayerMe.Tools.Editor.Cache
{
    public static class DeveloperDetailsCache
    {
        private const string Key = "RPM_DEVELOPER_DETAILS";

        public static Developer Data
        {
            get => JsonConvert.DeserializeObject<Developer>(EditorPrefs.GetString(Key));
            set => EditorPrefs.SetString(Key, JsonConvert.SerializeObject(value));
        }

        public static bool Exists()
        {
            return EditorPrefs.HasKey(Key);
        }
        
        public static void Delete()
        {
            EditorPrefs.DeleteKey(Key);
        }
    }
}
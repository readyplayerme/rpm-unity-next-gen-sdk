using Newtonsoft.Json;
using UnityEditor;

namespace ReadyPlayerMe.Editor.Cache
{
    public class DeveloperAuth
    {
        public string Token { get; set; }
        
        public string RefreshToken { get; set; }
        
        public string Name { get; set; }
    }
    
    public static class DeveloperAuthCache
    {
        private const string Key = "RPM_DEVELOPER_AUTH";

        public static DeveloperAuth Data
        {
            get => JsonConvert.DeserializeObject<DeveloperAuth>(EditorPrefs.GetString(Key));
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
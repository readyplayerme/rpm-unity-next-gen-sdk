using Newtonsoft.Json;

namespace ReadyPlayerMe.Editor.EditorPrefs
{
    public class DeveloperAuth
    {
        public string Token { get; set; }
        
        public string RefreshToken { get; set; }
        
        public string Name { get; set; }

        public bool IsDemo { get; set; }
    }
    
    public static class DeveloperAuthCache
    {
        private const string Key = "RPM_DEVELOPER_AUTH";

        public static DeveloperAuth Data
        {
            get => JsonConvert.DeserializeObject<DeveloperAuth>(UnityEditor.EditorPrefs.GetString(Key));
            set => UnityEditor.EditorPrefs.SetString(Key, JsonConvert.SerializeObject(value));
        }

        public static bool Exists()
        {
            return UnityEditor.EditorPrefs.HasKey(Key);
        }
        
        public static void Delete()
        {
            UnityEditor.EditorPrefs.DeleteKey(Key);
        }
    }
}
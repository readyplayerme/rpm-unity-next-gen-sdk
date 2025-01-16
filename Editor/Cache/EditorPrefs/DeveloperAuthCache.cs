using Newtonsoft.Json;
using UnityEngine;

namespace PlayerZero.Editor.Cache.EditorPrefs
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

        private static readonly string ProjectSpecificKey = Application.productName + "_" + Key;

        public static DeveloperAuth Data
        {
            get => JsonConvert.DeserializeObject<DeveloperAuth>(UnityEditor.EditorPrefs.GetString(ProjectSpecificKey));
            set => UnityEditor.EditorPrefs.SetString(ProjectSpecificKey, JsonConvert.SerializeObject(value));
        }

        public static bool Exists()
        {
            return UnityEditor.EditorPrefs.HasKey(ProjectSpecificKey);
        }
        
        public static void Delete()
        {
            UnityEditor.EditorPrefs.DeleteKey(ProjectSpecificKey);
        }
    }
}
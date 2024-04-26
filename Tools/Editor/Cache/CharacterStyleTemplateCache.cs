using System.Collections.Generic;
using Newtonsoft.Json;
using ReadyPlayerMe.Tools.Editor.Data;
using UnityEditor;

namespace ReadyPlayerMe.Tools.Editor.Cache
{
    public static class CharacterStyleTemplateCache
    {
        private const string Key = "RPM_CHARACTER_STYLE_TEMPLATES";

        public static IList<CharacterStyleTemplate> Data
        {
            get => JsonConvert.DeserializeObject<IList<CharacterStyleTemplate>>(EditorPrefs.GetString(Key)) ?? new List<CharacterStyleTemplate>();
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
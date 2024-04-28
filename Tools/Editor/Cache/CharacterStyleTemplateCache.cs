using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;

namespace ReadyPlayerMe.Tools.Editor.Cache
{
    public class CharacterStyleTemplate
    {
        public string Id { get; set; }
        
        public string CharacterStyleId { get; set; }
    }
    
    public static class CharacterStyleTemplateCache
    {
        private const string Key = "RPM_CHARACTER_STYLE_TEMPLATES";

        public static IList<CharacterStyleTemplate> Data
        {
            get => JsonConvert.DeserializeObject<IList<CharacterStyleTemplate>>(EditorPrefs.GetString(Key)) ?? new List<CharacterStyleTemplate>();
            set => EditorPrefs.SetString(Key, JsonConvert.SerializeObject(value));
        }
    }
}
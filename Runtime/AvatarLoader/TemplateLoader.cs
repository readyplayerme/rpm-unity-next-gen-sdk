using System.Linq;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class TemplateLoader
    {
        private static CharacterStyleTemplateConfig Config =>
            Resources.Load<CharacterStyleTemplateConfig>("CharacterStyleTemplateConfig");

        public static CharacterStyleTemplate GetById(string id)
        {
            return Config.templates.FirstOrDefault(p => p.id == id);
        }

        public static CharacterStyleTemplate GetByTag(string tag)
        {
            return Config.templates.FirstOrDefault(p => p.tags.Contains(tag));
        }
    }
}
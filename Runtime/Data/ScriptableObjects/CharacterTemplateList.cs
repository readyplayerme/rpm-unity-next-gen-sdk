using UnityEngine;

namespace ReadyPlayerMe.Data
{
    [CreateAssetMenu(fileName = "CharacterTemplateList", menuName = "Ready Player Me/CharacterTemplateList")]
    public class CharacterTemplateList : ScriptableObject
    {
        public CharacterTemplate[] templates;
        
        public static readonly string AssetName = "CharacterTemplateList";
    }
}

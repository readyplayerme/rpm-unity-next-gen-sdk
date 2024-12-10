using UnityEngine;

namespace ReadyPlayerMe.Data
{
    [CreateAssetMenu(fileName = "CharacterBlueprintTemplateList", menuName = "Ready Player Me/CharacterBlueprintTemplateList")]
    public class CharacterBlueprintTemplateList : ScriptableObject
    {
        public CharacterBlueprintTemplate[] templates;
    }
}

using System.Linq;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class CreatorCharacterLoader : MonoBehaviour
    {
        private const string BASE_MODEL_LABEL = "baseModel";
        private const string SKELETON_DEFINITION_LABEL = "SkeletonDefinitionConfig";
        private const string CHARACTER_STYLE_TEMPLATE_LABEL = "CharacterStyleTemplateConfig";
        
        private CharacterApi characterApi;
        private MeshTransfer meshTransfer;
        private SkeletonBuilder skeletonBuilder;
        
        private AssetLoader assetLoader;
        private CharacterLoader characterLoader;
        private string characterId;
        [SerializeField]
        private string styleId = "665e05e758e847063761c985";
        private CharacterData characterData;

        private GameObject CharacterObject;
        private void Start()
        {
            characterApi = new CharacterApi();
            meshTransfer = new MeshTransfer();
            skeletonBuilder = new SkeletonBuilder();
            characterLoader = new CharacterLoader();
            CreateCharacter(styleId);
        }

        public async void CreateCharacter(string templateTagOrId)
        {
            characterData = await characterLoader.LoadCharacter(templateTagOrId);
            characterId = characterData.Id;
            CharacterObject = characterData.gameObject;
        }

        public async void LoadAssetPreview(Asset asset)
        {
            if (asset.Type == "baseModel")
            {
                styleId = asset.Id;
            }
            characterData = await characterLoader.LoadAssetPreviewAsync(characterId, styleId, asset);
            if(CharacterObject != null)
            {
                Debug.Log( "Destroying old character object");
                Destroy(CharacterObject);
            }
            CharacterObject = characterData.gameObject;
        }
        
        protected virtual GameObject GetTemplate(string templateTagOrId)
        {
            if (string.IsNullOrEmpty(templateTagOrId))
                return null;

            return Resources
                .Load<CharacterStyleTemplateConfig>(CHARACTER_STYLE_TEMPLATE_LABEL)?
                .templates.FirstOrDefault(p => p.id == templateTagOrId || p.tags.Contains(templateTagOrId))?
                .template;
        }
    }
}
using GLTFast;
using System.Linq;
using UnityEngine;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using ReadyPlayerMe.Api;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe
{
    public class CharacterLoader 
    {
        private const string SKELETON_DEFINITION_LABEL = "SkeletonDefinitionConfig";
        
        private readonly CharacterApi _characterApi;
        private readonly MeshTransfer _meshTransfer;
        private readonly SkeletonBuilder _skeletonBuilder;
        private CharacterTemplateConfig templateConfig;
        private string applicationId;
        
        /// <summary>
        ///     Initializes a new instance of the CharacterLoader class.
        /// </summary>
        public CharacterLoader(CharacterTemplateConfig templateConfig = null)
        {
            _characterApi = new CharacterApi();
            _meshTransfer = new MeshTransfer();
            _skeletonBuilder = new SkeletonBuilder();
            this.templateConfig = templateConfig;
        }

        public async Task<CharacterData> LoadOnBlueprintAsync(string characterId, GameObject blueprint, GameObject meshParent = null, CharacterLoaderConfig config = null)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest { Id = characterId });
            var blueprintId = response.Data.BlueprintId;

            var characterData = blueprint.AddComponent<CharacterData>();
            characterData.Initialize(response.Data.Id, response.Data.BlueprintId);

            var characterObject = await LoadAndInstantiateCharacter(response.Data.ModelUrl, config, characterId);
            if (characterObject == null)
                return null;

            SetupCharacter(characterObject, blueprintId, characterData, meshParent ?? characterData.gameObject);

            return characterData;
        }

        public async Task<CharacterData> LoadAsync(string characterId, string tag = "", CharacterLoaderConfig config = null)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest { Id = characterId });
            var blueprintId = response.Data.BlueprintId;

            var templatePrefab = GetTemplate(blueprintId, tag);
            if (templatePrefab == null)
            {
                Debug.LogError($"Failed to load character template for character with ID {characterId}.");
                return null;
            }

            var templateInstance = Object.Instantiate(templatePrefab);
            var characterData = templateInstance.AddComponent<CharacterData>();
            characterData.Initialize(response.Data.Id, response.Data.BlueprintId);

            var characterObject = await LoadAndInstantiateCharacter(response.Data.ModelUrl, config, characterId);
            if (characterObject == null)
                return null;

            SetupCharacter(characterObject, blueprintId, characterData, characterData.gameObject);

            return characterData;
        }

        private async Task<GameObject> LoadAndInstantiateCharacter(string modelUrl, CharacterLoaderConfig config, string characterId)
        {
            config ??= new CharacterLoaderConfig();
            var query = QueryBuilder.BuildQueryString(config);
            var url = $"{modelUrl}?{query}";

            var gltf = new GltfImport();
            if (!await gltf.Load(url))
            {
                Debug.LogError($"Failed to load character model for character with ID {characterId}.");
                return null;
            }

            var characterObject = new GameObject(characterId);
            gltf.InstantiateSceneAsync(characterObject.transform).Wait();

            return characterObject;
        }

        private void SetupCharacter(GameObject characterObject, string blueprintId, CharacterData characterData, GameObject meshParent)
        {
            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == blueprintId)?.definition;

            var animator = characterData.gameObject.GetComponent<Animator>() ?? characterData.gameObject.AddComponent<Animator>();
            animator.enabled = false;

            if (animator.avatar == null)
            {
                _skeletonBuilder.Build(characterData.gameObject, skeletonDefinition?.GetHumanBones());
            }

            _meshTransfer.Transfer(characterObject, meshParent);
            characterData.gameObject.SetActive(true);

            animator.enabled = true;
        }
        
        /// <summary>
        ///     Retrieves a template based on the given template tag or ID.
        /// </summary>
        /// <param name="templateTagOrId"> The template tag or ID of the character to load. </param>
        /// <returns> A GameObject representing the template. </returns>
        protected virtual GameObject GetTemplate(string blueprintId, string tag = "")
        {
            if (string.IsNullOrEmpty(blueprintId))
                return null;

            if (templateConfig == null) // load default if not set
            {
                if (string.IsNullOrEmpty(applicationId))
                {
                    applicationId = Resources.Load<Settings>( "ReadyPlayerMeSettings")?.ApplicationId;
                }
                templateConfig = Resources.Load<CharacterTemplateConfig>(applicationId);
            }
            if (templateConfig == null)
            {
                Debug.LogError("Character template config not found.");
                return null;
            }
            var blueprintTemplate = templateConfig.Templates.ToList().FirstOrDefault(p => p.BlueprintId == blueprintId) ?? templateConfig.Templates[0];
            return blueprintTemplate.GetPrefabByTag(tag);
        }
    }
}
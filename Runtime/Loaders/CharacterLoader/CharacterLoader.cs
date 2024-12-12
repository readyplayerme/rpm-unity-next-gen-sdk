using System;
using GLTFast;
using System.Linq;
using UnityEngine;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe
{
    public class CharacterLoader
    {
        private const string BASE_MODEL_LABEL = "baseModel";
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

        /// <summary>
        ///     Asynchronously loads a character based on the given template tag or ID.
        /// </summary>
        /// <param name="templateTagOrId"> The template tag or ID of the character to load. </param>
        /// <param name="useCache"> A boolean indicating whether to use cached data if available. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a CharacterData object. </returns>
        public async Task<CharacterData> LoadCharacter(string templateTagOrId, bool useCache = false)
        {
            CharacterData characterData;
            
            if (useCache)
            {
                // TODO: get or create cached character id
                characterData = LoadTemplate(templateTagOrId);
            }
            else
            {
                var createResponse = await _characterApi.CreateAsync(new CharacterCreateRequest()
                {
                    Payload = new CharacterCreateRequestBody()
                    {
                        Assets = new Dictionary<string, string>
                        {
                            { BASE_MODEL_LABEL, templateTagOrId }
                        }
                    }
                });
                
                characterData = LoadTemplate(templateTagOrId, createResponse.Data.Id);
            }
            
            characterData.gameObject.SetActive(false);
            
            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == templateTagOrId)?
                .definition;

            characterData.gameObject.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;
            
            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                _skeletonBuilder.Build(characterData.gameObject, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }
                
            animator.enabled = true;
            
            characterData.gameObject.SetActive(true);
                
            return characterData;
        }
        
        /// <summary>
        ///   Asynchronously loads a character based on the given character ID, template tag or ID, and asset.
        /// </summary>
        /// <param name="characterId"> The ID of the character to load. </param>
        /// <param name="templateTagOrId"> The template tag or ID of the character to load. </param>
        /// <param name="asset"> The asset to load. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a CharacterData object. </returns>
        public async Task<CharacterData> LoadAsync(string characterId, string templateTagOrId, Asset asset)
        {
            await _characterApi.UpdateAsync(new CharacterUpdateRequest()
            {
                Id = characterId,
                Payload = new CharacterUpdateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { asset.Type, asset.Id }
                    }
                }
            });
            
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = characterId,
            });
            
            Character character = response.Data;
            CharacterData characterData = LoadTemplate(templateTagOrId);

            characterData.gameObject.SetActive(false);
            
            var gltf = new GltfImport();

            if (!await gltf.Load(character.ModelUrl))
                return null;

            var characterObject = new GameObject(character.Id);

            await gltf.InstantiateSceneAsync(characterObject.transform);

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == templateTagOrId)?
                .definition;

            characterData.gameObject.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;
            
            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                _skeletonBuilder.Build(characterData.gameObject, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }
                
            _meshTransfer.Transfer(characterObject, characterData.gameObject);
            characterData.gameObject.SetActive(true);
                
            animator.enabled = true;
            
            return characterData;
        }


        public async Task<CharacterData> LoadCharacterAsync(string characterId)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = characterId,
            });
            var blueprintId = response.Data.BlueprintId;
            var templatePrefab = GetTemplate(blueprintId);

            var templateInstance = templatePrefab != null ? Object.Instantiate(templatePrefab) : null;
            if (templateInstance == null)
            {
                Debug.LogError( $"Failed to load character template for character with ID {characterId}." );
                return null;
            }

            var characterData = templateInstance.AddComponent<CharacterData>();
            characterData.Initialize(response.Data.Id, response.Data.BlueprintId);
            var gltf = new GltfImport();

            if (!await gltf.Load(response.Data.ModelUrl))
            {
                Debug.LogError( $"Failed to load character model for character with ID {characterId}." );
                return null;
            }
            
            var characterObject = new GameObject(characterId);

            await gltf.InstantiateSceneAsync(characterObject.transform);

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == blueprintId)?
                .definition;

            characterData.gameObject.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;
        
            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                _skeletonBuilder.Build(characterData.gameObject, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }
            
            _meshTransfer.Transfer(characterObject, characterData.gameObject);
            characterData.gameObject.SetActive(true);
            
            animator.enabled = true;
        
            return characterData;

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

        /// <summary>
        ///    Loads a template based on the given template tag or ID.
        /// </summary>
        /// <param name="templateTagOrId"> The template tag or ID of the character to load. </param>
        /// <param name="characterId"> The ID of the character to load. </param>
        /// <returns> A CharacterData object representing the loaded template. </returns>
        private CharacterData LoadTemplate(string templateTagOrId, string characterId = null)
        {
            GameObject templatePrefab = GetTemplate(templateTagOrId);
            GameObject templateInstance = templatePrefab != null ? Object.Instantiate(templatePrefab) : null;

            var data = templateInstance?.GetComponent<CharacterData>();

            if (data == null)
                data = templateInstance?.AddComponent<CharacterData>();
            
            //data?.Initialize(characterId, templateTagOrId);

            return data;
        }
    }
}
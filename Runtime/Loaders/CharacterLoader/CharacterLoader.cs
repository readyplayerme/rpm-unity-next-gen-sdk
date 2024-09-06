using GLTFast;
using System.Linq;
using UnityEngine;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public class CharacterLoader
    {
        private readonly CharacterApi _characterApi;
        private readonly MeshTransfer _meshTransfer;
        private readonly SkeletonBuilder _skeletonBuilder;
        
        public CharacterLoader()
        {
            _characterApi = new CharacterApi();
            _meshTransfer = new MeshTransfer();
            _skeletonBuilder = new SkeletonBuilder();
        }

        // this is a new method
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
                            { "baseModel", templateTagOrId }
                        }
                    }
                });
                
                characterData = LoadTemplate(templateTagOrId, createResponse.Data.Id);
            }
            
            characterData.gameObject.SetActive(false);
            
            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig")
                .definitionLinks
                .FirstOrDefault(p => p.characterStyleId == templateTagOrId)?
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
        
        public async Task<CharacterData> LoadAsyncX(string characterId, string templateTagOrId, Asset asset)
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
            CharacterData characterData = LoadTemplate(templateTagOrId, character.Id);
            characterData.gameObject.SetActive(false);
            
            var gltf = new GltfImport();

            if (!await gltf.Load(character.GlbUrl))
                return null;

            var characterObject = new GameObject(character.Id);

            await gltf.InstantiateSceneAsync(characterObject.transform);

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig")
                .definitionLinks
                .FirstOrDefault(p => p.characterStyleId == templateTagOrId)?
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
        
        protected virtual GameObject GetTemplate(string templateTagOrId)
        {
            if (string.IsNullOrEmpty(templateTagOrId))
                return null;

            return Resources
                .Load<CharacterStyleTemplateConfig>($"CharacterStyleTemplateConfig")?
                .templates.FirstOrDefault(p => p.id == templateTagOrId || p.tags.Contains(templateTagOrId))?
                .template;
        }

        private CharacterData LoadTemplate(string templateTagOrId, string characterId = null)
        {
            GameObject template = GetTemplate(templateTagOrId);
            GameObject templateInstance = template != null ? Object.Instantiate(template) : null;

            var data = templateInstance?.GetComponent<CharacterData>();

            if (data == null)
                data = templateInstance?.AddComponent<CharacterData>();
            
            data?.Initialize(characterId, templateTagOrId);

            return data;
        }
    }
}
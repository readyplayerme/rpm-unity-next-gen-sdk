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

        public virtual Task<CharacterData> PreviewAsync(
            string id,
            Dictionary<string, string> assets,
            string templateTagOrId = null
        )
        {
            var template = GetTemplate(templateTagOrId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;
            templateInstance?.SetActive(false);

            return PreviewAsync(id, assets, templateInstance);
        }

        public virtual async Task<CharacterData> PreviewAsync(
            string id,
            Dictionary<string, string> assets,
            GameObject template = null
        )
        {
            assets.TryGetValue("baseModel", out var styleId);

            if (string.IsNullOrEmpty(styleId))
            {
                var characterResponse = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
                {
                    Id = id,
                });

                styleId = characterResponse.Data.Assets["baseModel"];
            }

            var previewUrl = _characterApi.GeneratePreviewUrl(new CharacterPreviewRequest()
            {
                Id = id,
                Params = new CharacterPreviewQueryParams()
                {
                    Assets = assets
                }
            });

            return await LoadAsync(id, styleId, previewUrl, template);
        }

        public virtual async Task<CharacterData> LoadAsync(string id)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = id,
            });

            return await LoadAsync(
                response.Data.Id,
                response.Data.Assets["baseModel"],
                response.Data.GlbUrl,
                null
            );
        }

        public virtual async Task<CharacterData> LoadAsync(string id, string templateTagOrId)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = id,
            });

            var template = GetTemplate(templateTagOrId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;
            templateInstance?.SetActive(false);

            return await LoadAsync(
                response.Data.Id,
                response.Data.Assets["baseModel"],
                response.Data.GlbUrl,
                templateInstance
            );
        }

        public virtual async Task<CharacterData> LoadAsync(
            string id,
            string styleId,
            string loadFrom,
            GameObject template
        )
        {
            var gltf = new GltfImport();

            if (!await gltf.Load(loadFrom))
                return null;

            var character = new GameObject(id);

            await gltf.InstantiateSceneAsync(character.transform);

            if (template == null)
                return InitCharacter(character, id, styleId);

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig")
                .definitionLinks
                .FirstOrDefault(p => p.characterStyleId == styleId)?
                .definition;

            // Update skeleton and transfer mesh
            template.TryGetComponent<Animator>(out var animator);
            var animationAvatar = animator != null ? animator.avatar : null;
            if (animationAvatar == null)
            {
                _skeletonBuilder.Build(template, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }

            _meshTransfer.Transfer(character, template);

            return InitCharacter(template, id, styleId);
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

        protected virtual CharacterData InitCharacter(GameObject character, string id, string styleId)
        {
            var data = character.GetComponent<CharacterData>();

            if (data == null)
                data = character.AddComponent<CharacterData>();
            
            character.SetActive(true);

            return data.Initialize(id, styleId);
        }
    }
}
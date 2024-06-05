using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.CharacterLoader
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

            return PreviewAsync(id, assets, templateInstance);
        }

        public virtual async Task<CharacterData> PreviewAsync(
            string id,
            Dictionary<string, string> assets,
            GameObject template = null
        )
        {
            var previewUrl = _characterApi.GeneratePreviewUrl(new CharacterPreviewRequest()
            {
                Id = id,
                Params = new CharacterPreviewQueryParams()
                {
                    Assets = assets
                }
            });

            return await LoadAsync(id, template, previewUrl);
        }

        public virtual async Task<CharacterData> LoadAsync(string id, string templateTagOrId = null)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = id,
            });

            var template = GetTemplate(templateTagOrId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;

            return await LoadAsync(response.Data.Id, templateInstance, response.Data.GlbUrl);
        }

        public virtual async Task<CharacterData> LoadAsync(
            string id,
            GameObject template = null,
            string loadFrom = null,
            string styleId = null
        )
        {
            if (string.IsNullOrEmpty(loadFrom) || string.IsNullOrEmpty(styleId))
            {
                var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
                {
                    Id = id,
                });

                if (string.IsNullOrEmpty(loadFrom))
                    loadFrom = response.Data.GlbUrl;

                if (string.IsNullOrEmpty(styleId))
                    styleId = response.Data.Assets["baseModel"];
            }

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

            return data.Initialize(id, styleId);
        }
    }
}
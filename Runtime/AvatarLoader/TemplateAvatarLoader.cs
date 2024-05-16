using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class TemplateAvatarLoader
    {
        private readonly AvatarApi _avatarApi;
        private readonly AvatarLoader _avatarLoader;

        public TemplateAvatarLoader()
        {
            _avatarApi = new AvatarApi();
            _avatarLoader = new AvatarLoader();
        }

        public virtual async Task<GameObject> PreviewAsync(
            string id,
            Dictionary<string, string> assets,
            string templateId = null
        )
        {
            var template = GetTemplate(templateId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;

            return await _avatarLoader.PreviewAsync(id, assets, templateInstance);
        }

        public virtual async Task<GameObject> LoadAsync(string id, string templateId = null)
        {
            var avatarResponse = await _avatarApi.FindAvatarByIdAsync(new AvatarFindByIdRequest()
            {
                AvatarId = id,
            });

            var template = GetTemplate(templateId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;

            return await _avatarLoader
                .LoadAsync(avatarResponse.Data.Id, templateInstance, avatarResponse.Data.GlbUrl);
        }

        protected virtual GameObject GetTemplate(string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;

            return Resources
                .Load<CharacterStyleTemplateConfig>($"CharacterStyleTemplateConfig")?
                .templates.FirstOrDefault(p => p.id == templateId)?
                .template;
        }
    }
}
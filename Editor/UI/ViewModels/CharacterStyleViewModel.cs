using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Cache;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        public Asset CharacterStyle { get; private set; }

        public string AvatarBoneDefinitionCacheId { get; private set; }

        public Texture2D Image { get; private set; }

        private ObjectCache<AvatarSkeletonDefinition>
            _avatarSkeletonDefinitionObjectCache;

        private GlbCache _characterStyleCache;

        private FileApi _fileApi;

        public async Task Init(Asset characterStyle)
        {
            _avatarSkeletonDefinitionObjectCache =
                new ObjectCache<AvatarSkeletonDefinition>("Character Avatar Bone Definitions");

            _characterStyleCache = new GlbCache("Character Templates");

            CharacterStyle = characterStyle;

            AvatarBoneDefinitionCacheId = Resources
                .Load<AvatarSkeletonDefinition>($"Character Avatar Bone Definitions/{CharacterStyle.Id}")?.cacheId;

            _fileApi = new FileApi();
            Image = await _fileApi.DownloadImageAsync(CharacterStyle.IconUrl);
        }

        public async Task LoadStyleAsync()
        {
            var bytes = await _fileApi.DownloadFileIntoMemoryAsync(CharacterStyle.GlbUrl);

            await _characterStyleCache.Save(bytes, CharacterStyle.Id);

            var character = _characterStyleCache.Load(CharacterStyle.Id);
            var instance = PrefabUtility.InstantiatePrefab(character) as GameObject;
            var skeletonBuilder = new SkeletonBuilder();
            var avatarSkeletonDefinition = Resources
                .Load<AvatarSkeletonDefinition>($"Character Avatar Bone Definitions/{CharacterStyle.Id}");

            if (avatarSkeletonDefinition == null)
                return;

            skeletonBuilder.Build(instance, avatarSkeletonDefinition?.GetHumanBones());
        }

        /* public void SaveTemplate(GameObject templateObject)
        {
            if (templateObject == null)
            {
                _characterStyleTemplateScriptableObjectCacheWriter.Delete(CharacterStyle.Id);
                return;
            }

            var template = ScriptableObject.CreateInstance<CharacterStyleTemplateReference>();
            template.characterStyleTemplate = templateObject;
            template.cacheId = FindAssetGuid(templateObject);
            _characterStyleTemplateScriptableObjectCacheWriter.Save(template, CharacterStyle.Id);
        } */

        public void SaveAvatarBoneDefinition(AvatarSkeletonDefinition avatarBoneDefinitionObject)
        {
            avatarBoneDefinitionObject.cacheId = Cache.Cache.FindAssetGuid(avatarBoneDefinitionObject);
            _avatarSkeletonDefinitionObjectCache.Save(avatarBoneDefinitionObject, CharacterStyle.Id);
        }
    }
}
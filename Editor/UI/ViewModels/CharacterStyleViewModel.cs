using System;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Cache;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Data.V1;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        public Asset CharacterStyle { get; set; }

        public string TemplateCacheId { get; private set; }
        public string AvatarBoneDefinitionCacheId { get; private set; }

        public Texture2D Image { get; private set; }

        private CharacterStyleCache _characterStyleCache;
        private CharacterDataCache<GameObject> _characterStyleTemplateCache;
        private CharacterDataCache<AvatarSkeletonDefinition> _characterAvatarBoneDefinitionCache;

        private FileApi _fileApi;

        public async Task Init(Asset characterStyle)
        {
            _characterStyleCache = new CharacterStyleCache();
            _characterStyleTemplateCache = new CharacterDataCache<GameObject>("Character Templates Links");
            _characterAvatarBoneDefinitionCache = new CharacterDataCache<AvatarSkeletonDefinition>("Character Avatar Bone Definitions");
                
            CharacterStyle = characterStyle;
            TemplateCacheId = _characterStyleTemplateCache.GetCacheId(CharacterStyle.Id);
            AvatarBoneDefinitionCacheId = _characterAvatarBoneDefinitionCache.GetCacheId(CharacterStyle.Id);

            _fileApi = new FileApi();
            Image = await _fileApi.DownloadImageAsync(CharacterStyle.IconUrl);
        }

        public async Task LoadStyleAsync()
        {
            try
            {
                var bytes = await _fileApi.DownloadFileIntoMemoryAsync(CharacterStyle.GlbUrl);

                await _characterStyleCache.Save(bytes, CharacterStyle.Id);

                var character = _characterStyleCache.Load(CharacterStyle.Id);
                var avatarSkeletonDefinition = _characterAvatarBoneDefinitionCache.Load(CharacterStyle.Id);

                var instance = PrefabUtility.InstantiatePrefab(character) as GameObject;
                
                SkeletonBuilder skeletonBuilder = new SkeletonBuilder();
                
                skeletonBuilder.Build(instance, avatarSkeletonDefinition?.GetHumanBones());
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void SaveTemplate(GameObject templateObject)
        {
            _characterStyleTemplateCache.Save(templateObject, CharacterStyle.Id);
        }
        
        public void SaveAvatarBoneDefinition(AvatarSkeletonDefinition avatarBoneDefinitionObject)
        {
            _characterAvatarBoneDefinitionCache.Save(avatarBoneDefinitionObject, CharacterStyle.Id);
        }
    }
}
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Cache;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        public Asset CharacterStyle { get; private set; }

        public string TemplateCacheId { get; private set; }

        public string AvatarBoneDefinitionCacheId { get; private set; }

        public Texture2D Image { get; private set; }

        private ScriptableObjectCacheWriter<CharacterStyleTemplateReference>
            _characterStyleTemplateScriptableObjectCacheWriter;

        private ScriptableObjectCacheWriter<AvatarSkeletonDefinition>
            _avatarSkeletonDefinitionScriptableObjectCacheWriter;

        private GlbCache _characterStyleCache;

        private FileApi _fileApi;

        public async Task Init(Asset characterStyle)
        {
            _characterStyleTemplateScriptableObjectCacheWriter =
                new ScriptableObjectCacheWriter<CharacterStyleTemplateReference>("Character Templates Links");

            _avatarSkeletonDefinitionScriptableObjectCacheWriter =
                new ScriptableObjectCacheWriter<AvatarSkeletonDefinition>("Character Avatar Bone Definitions");

            _characterStyleCache = new GlbCache("Character Templates");

            CharacterStyle = characterStyle;
            
            TemplateCacheId = Resources
                .Load<CharacterStyleTemplateReference>($"Character Templates Links/{CharacterStyle.Id}")?.cacheId;
            
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
            var avatarSkeletonDefinition = Resources
                .Load<AvatarSkeletonDefinition>($"Character Avatar Bone Definitions/{CharacterStyle.Id}");

            if (avatarSkeletonDefinition == null)
                return;

            var skeletonBuilder = new SkeletonBuilder();
            skeletonBuilder.Build(instance, avatarSkeletonDefinition.GetHumanBones());
        }

        public void SaveTemplate(GameObject templateObject)
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
        }

        public void SaveAvatarBoneDefinition(AvatarSkeletonDefinition avatarBoneDefinitionObject)
        {
            avatarBoneDefinitionObject.cacheId = FindAssetGuid(avatarBoneDefinitionObject);
            _avatarSkeletonDefinitionScriptableObjectCacheWriter.Save(avatarBoneDefinitionObject, CharacterStyle.Id);
        }

        private string FindAssetGuid(Object asset)
        {
            var guids = new NativeArray<GUID>(new GUID[1]
                {
                    GUID.Generate(),
                },
                Allocator.Temp
            );

            AssetDatabase.InstanceIDsToGUIDs(
                new NativeArray<int>(new int[] { asset.GetInstanceID() }, Allocator.Temp),
                guids
            );

            Debug.Log(guids[0]);

            return guids[0].ToString();
        }
    }
}
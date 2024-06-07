using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.CharacterLoader;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Cache;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterStyleViewModel
    {
        public Asset CharacterStyle { get; private set; }

        public string BoneDefinitionCacheId { get; private set; }

        public Texture2D Image { get; private set; }

        private SkeletonDefinitionConfig
            _skeletonDefinitionObjectCache;

        private GlbCache _characterStyleCache;

        public async Task Init(Asset characterStyle)
        {
            _skeletonDefinitionObjectCache =
                Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig");

            _characterStyleCache = new GlbCache("Character Templates");

            CharacterStyle = characterStyle;

            BoneDefinitionCacheId = _skeletonDefinitionObjectCache.definitionLinks
                .FirstOrDefault(p => p.characterStyleId == characterStyle.Id)?.definitionCacheId;
            
            Image = await FileApi.DownloadImageAsync(CharacterStyle.IconUrl);
        }

        public async Task LoadStyleAsync()
        {
            var bytes = await FileApi.DownloadFileIntoMemoryAsync(CharacterStyle.GlbUrl);

            await _characterStyleCache.Save(bytes, CharacterStyle.Id);

            var character = _characterStyleCache.Load(CharacterStyle.Id);
            var instance = PrefabUtility.InstantiatePrefab(character) as GameObject;
            var skeletonBuilder = new SkeletonBuilder();
            var skeletonDefinition = _skeletonDefinitionObjectCache.definitionLinks
                .FirstOrDefault(p => p.characterStyleId == CharacterStyle.Id)?
                .definition;

            if (skeletonDefinition == null)
                return;

            skeletonBuilder.Build(instance, skeletonDefinition.GetHumanBones());
        }

        public void SaveBoneDefinition(SkeletonDefinition skeletonDefinitionObject)
        {
            var skeletonDefinitionConfig =
                Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig");
            var definitionList = skeletonDefinitionConfig.definitionLinks.ToList();
            var existingDefinitions = definitionList
                .Where(p => p.characterStyleId != CharacterStyle.Id)
                .ToList();

            if (skeletonDefinitionObject != null)
            {
                var definition = new SkeletonDefinitionLink()
                {
                    definition = skeletonDefinitionObject,
                    characterStyleId = CharacterStyle.Id,
                    definitionCacheId = Cache.Cache.FindAssetGuid(skeletonDefinitionObject)
                };

                existingDefinitions.Add(definition);
            }

            skeletonDefinitionConfig.definitionLinks = existingDefinitions.ToArray();

            EditorUtility.SetDirty(skeletonDefinitionConfig);
            AssetDatabase.Refresh();
        }
    }
}
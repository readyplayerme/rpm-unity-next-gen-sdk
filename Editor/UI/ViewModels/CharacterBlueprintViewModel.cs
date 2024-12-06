using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Editor.Api.V1.Analytics;
using ReadyPlayerMe.Editor.Api.V1.Analytics.Models;
using ReadyPlayerMe.Editor.Cache;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor.UI.ViewModels
{
    public class CharacterBlueprintViewModel
    {
        public Asset CharacterBlueprint { get; private set; }

        public string BoneDefinitionCacheId { get; private set; }

        public Texture2D Image { get; private set; }

        private SkeletonDefinitionConfig
            _skeletonDefinitionObjectCache;

        private GlbCache _characterBlueprintCache;
        
        private readonly AnalyticsApi _analyticsApi;
        private readonly FileApi _fileApi;
        
        public CharacterBlueprintViewModel(AnalyticsApi analyticsApi)
        {
            _fileApi = new FileApi();
            _analyticsApi = analyticsApi;
        }

        public async Task Init(Asset characterBlueprint)
        {
            _skeletonDefinitionObjectCache =
                Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig");

            _characterBlueprintCache = new GlbCache("Character Blueprints");

            CharacterBlueprint = characterBlueprint;

            BoneDefinitionCacheId = _skeletonDefinitionObjectCache.definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == characterBlueprint.Id)?.definitionCacheId;
            
            Image = await _fileApi.DownloadImageAsync(CharacterBlueprint.IconUrl);
        }

        public async Task LoadBlueprintAsync()
        {
            var bytes = await _fileApi.DownloadFileIntoMemoryAsync(CharacterBlueprint.GlbUrl);

            await _characterBlueprintCache.Save(bytes, CharacterBlueprint.Id);

            var character = _characterBlueprintCache.Load(CharacterBlueprint.Id);
            var instance = PrefabUtility.InstantiatePrefab(character) as GameObject;
            var skeletonBuilder = new SkeletonBuilder();
            var skeletonDefinition = _skeletonDefinitionObjectCache.definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == CharacterBlueprint.Id)?
                .definition;
            
            _analyticsApi.SendEvent(new AnalyticsEventRequest()
            {
                Payload = new AnalyticsEventRequestBody()
                {
                    Event = "next gen unity sdk action",
                    Properties =
                    {
                        { "type", "Import Character Blueprint" },
                        { "blueprintId", CharacterBlueprint.Id }
                    }
                }
            });

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
                .Where(p => p.characterBlueprintId != CharacterBlueprint.Id)
                .ToList();

            if (skeletonDefinitionObject != null)
            {
                var definition = new SkeletonDefinitionLink()
                {
                    definition = skeletonDefinitionObject,
                    characterBlueprintId = CharacterBlueprint.Id,
                    definitionCacheId = Cache.Cache.FindAssetGuid(skeletonDefinitionObject)
                };

                existingDefinitions.Add(definition);
            }

            skeletonDefinitionConfig.definitionLinks = existingDefinitions.ToArray();

            EditorUtility.SetDirty(skeletonDefinitionConfig);
            AssetDatabase.Refresh();
            
            _analyticsApi.SendEvent(new AnalyticsEventRequest()
            {
                Payload = new AnalyticsEventRequestBody()
                {
                    Event = "next gen unity sdk action",
                    Properties =
                    {
                        { "type", "Save Skeleton Definition" },
                        { "blueprintId", CharacterBlueprint.Id }
                    }
                }
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GLTFast;
using Newtonsoft.Json;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class CreatorCharacterLoader : MonoBehaviour
    {
        [SerializeField]
        private string styleId = "665e05e758e847063761c985";
        
        private const string BASE_MODEL_LABEL = "baseModel";
        private const string SKELETON_DEFINITION_LABEL = "SkeletonDefinitionConfig";
        private const string CHARACTER_STYLE_TEMPLATE_LABEL = "CharacterStyleTemplateConfig";
        
        private CharacterApi characterApi;
        private MeshTransfer meshTransfer;
        private SkeletonBuilder skeletonBuilder;
        
        private AssetLoader assetLoader;
        private CharacterLoader characterLoader;
        private string characterId;
        private CharacterData characterData;
        private GameObject CharacterObject;
        
        public UnityEvent<GameObject> OnCharacterLoaded;
        
        private AssetApi assetApi;
        
        public Dictionary<string, Asset> AssetsMap = new Dictionary<string, Asset>();
        public Dictionary<string, SkinnedMeshRenderer[]> AssetMeshMap = new Dictionary<string, SkinnedMeshRenderer[]>();
        [SerializeField]
        private bool useCache;
        
        Asset[] cachedAssets;
        
        private async void Start()
        {
            //disable if cache doesn't exist
            useCache == useCache && File.Exists(CachePaths.CACHE_ASSET_JSON_PATH);
            characterApi = new CharacterApi();
            meshTransfer = new MeshTransfer();
            skeletonBuilder = new SkeletonBuilder();
            characterLoader = new CharacterLoader();
            assetApi = new AssetApi();
            var createResponse = await characterApi.CreateAsync(new CharacterCreateRequest()
            {
                Payload = new CharacterCreateRequestBody()
                {
                    Assets = new Dictionary<string, string>
                    {
                        { BASE_MODEL_LABEL, styleId }
                    }
                }
            });
            characterId = createResponse.Data.Id;
            LoadNewCharacter();
            await GetCachedAssets();
        }

        private void LoadNewCharacter()
        {
            if(CharacterObject != null)
            {
                Destroy(CharacterObject);
                AssetMeshMap.Clear();
            }
            CharacterObject = Instantiate(GetTemplate(styleId));
            AssetsMap[BASE_MODEL_LABEL] = new Asset {Id = styleId, Type = BASE_MODEL_LABEL};
            var skinnedMeshes = CharacterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            AssetMeshMap[BASE_MODEL_LABEL] = skinnedMeshes;
            
            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterStyleId == styleId)?
                .definition;

            CharacterObject.gameObject.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;
            
            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                skeletonBuilder.Build(CharacterObject.gameObject, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }
            
            animator.enabled = true;
        }

        public async void CreateCharacter(string templateTagOrId)
        {
            characterData = await characterLoader.LoadCharacter(templateTagOrId);
            characterId = characterData.Id;
            CharacterObject = characterData.gameObject;
            OnCharacterLoaded?.Invoke(CharacterObject);
        }

        private async Task GetCachedAssets()
        {
            if (useCache && File.Exists(CachePaths.CACHE_ASSET_JSON_PATH))
            {
                cachedAssets = await LoadAssetsFromCacheAsync(styleId);
            }
        }

        private async Task<Asset[]> LoadAssetsFromCacheAsync(string characterModelAssetId)
        {
            if (!File.Exists(CachePaths.CACHE_ASSET_JSON_PATH))
                throw new FileNotFoundException("Cache file not found.", CachePaths.CACHE_ASSET_JSON_PATH);
        
            string json = await File.ReadAllTextAsync(CachePaths.CACHE_ASSET_JSON_PATH);
            CachedAsset[] cachedAssets = JsonConvert.DeserializeObject<CachedAsset[]>(json);
            Asset[] assets = cachedAssets.Select(cachedAsset =>
            {
                Asset asset = cachedAsset;
                if(characterModelAssetId != null) 
                    asset.GlbUrl = cachedAsset.GlbUrls[characterModelAssetId];
                
                return asset;
            }).ToArray();
            
            return assets;
        }

        public async void RemoveAsset(Asset asset)
        {
            if (asset.Type == "baseModel") return;
            AssetsMap.Remove(asset.Type);
            if(AssetMeshMap.ContainsKey(asset.Type))
            {
                foreach (var skinnedMesh in AssetMeshMap[asset.Type])
                {
                    Destroy(skinnedMesh.gameObject);
                }
                AssetMeshMap.Remove(asset.Type);
            }
        }
        
        public async void LoadAssetPreview(Asset asset)
        {
            if (asset.Type == "baseModel")
            {
                styleId = asset.Id;
                LoadNewCharacter();
                
                var assetArray = AssetsMap.Values.ToArray();
                foreach (var assetFromArray in assetArray)
                {
                    if(assetFromArray.Type == BASE_MODEL_LABEL)
                    {
                        continue;
                    }
                    LoadAssetPreview(assetFromArray);
                }
                //reload assets
                return;
            }
            var gltf = new GltfImport();
            var outfit = new GameObject(asset.Id);
            AssetsMap[asset.Type] = asset;

            if (useCache)
            {
                var path = $"{CachePaths.CACHE_ASSET_ROOT}/{styleId}/{asset.Id}";
                byte[] assetBytes = await File.ReadAllBytesAsync(path);
                await gltf.Load(assetBytes);
                await gltf.InstantiateSceneAsync(outfit.transform);
                var NewSkinnedMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
                if(AssetMeshMap.ContainsKey(asset.Type))
                {
                    foreach (var skinnedMesh in AssetMeshMap[asset.Type])
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                    AssetMeshMap.Remove(asset.Type);
                }
                AssetMeshMap[asset.Type] = NewSkinnedMeshes;
                meshTransfer.TransferMeshes(CharacterObject.transform, outfit.transform, CharacterObject.transform);

            }
            else
            {
                var assets = new Dictionary<string, string>();
                foreach (var AssetInMap in AssetsMap)
                {
                    assets[AssetInMap.Value.Type] = AssetInMap.Value.Id;
                }
                var url = characterApi.GeneratePreviewUrl(new CharacterPreviewRequest()
                {
                    Id = characterId,
                    Params = new CharacterPreviewQueryParams()
                    {
                        Assets = assets,
                    }
                });
                Debug.Log($"Url for preview: {url}");
                await gltf.Load(url);
                await gltf.InstantiateSceneAsync(outfit.transform);
                var NewSkinnedMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
                Debug.Log($" Number of skinned meshes added: {NewSkinnedMeshes.Length}");
                
                meshTransfer.TransferMeshes(CharacterObject.transform, outfit.transform, CharacterObject.transform);
                foreach (var AssetMeshMap in AssetMeshMap)
                {
                    foreach (var skinnedMesh in AssetMeshMap.Value)
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                }
                AssetMeshMap.Clear();
                AssetMeshMap[string.Empty] = NewSkinnedMeshes;
            }

            Destroy(outfit);
            OnCharacterLoaded?.Invoke(CharacterObject);
        }

        public async void LoadAssetPreviewOld(Asset asset)
        {
            if (asset.Type == "baseModel")
            {
                styleId = asset.Id;
            }

            characterData = await characterLoader.LoadAssetPreviewAsync(characterId, styleId, asset);
            //characterData = await characterLoader.LoadAsync(characterId, styleId, asset);

            if(CharacterObject != null)
            {
                Debug.Log( "Destroying old character object");
                Destroy(CharacterObject);
            }
            CharacterObject = characterData.gameObject;
            OnCharacterLoaded?.Invoke(CharacterObject);
        }
        
        protected virtual GameObject GetTemplate(string templateTagOrId)
        {
            if (string.IsNullOrEmpty(templateTagOrId))
                return null;

            return Resources
                .Load<CharacterStyleTemplateConfig>(CHARACTER_STYLE_TEMPLATE_LABEL)?
                .templates.FirstOrDefault(p => p.id == templateTagOrId || p.tags.Contains(templateTagOrId))?
                .template;
        }
    }
}
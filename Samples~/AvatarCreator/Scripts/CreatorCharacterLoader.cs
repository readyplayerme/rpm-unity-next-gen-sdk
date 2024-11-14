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
        
        private const string STYLE_ASSET_LABEL = "baseModel";
        private const string SKELETON_DEFINITION_LABEL = "SkeletonDefinitionConfig";
        private const string CHARACTER_STYLE_TEMPLATE_LABEL = "CharacterStyleTemplateConfig";
        
        private CharacterApi characterApi;
        private MeshTransfer meshTransfer;
        private SkeletonBuilder skeletonBuilder;
        
        private AssetLoader assetLoader;
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
        private GameObject loadingObject;
        private bool isUpdatingBaseModel;

        private async void Start()
        {
            loadingObject = new GameObject("LoadingMeshes");
            loadingObject.SetActive(false);
            if (useCache && !File.Exists(CachePaths.CACHE_ASSET_JSON_PATH))
            {
                var cacheGenerator = new CacheGenerator();
                cacheGenerator.ExtractCache();
            }
            //disable if cache doesn't exist
            useCache = useCache && File.Exists(CachePaths.CACHE_ASSET_JSON_PATH);
            
            characterApi = new CharacterApi();
            meshTransfer = new MeshTransfer();
            skeletonBuilder = new SkeletonBuilder();
            assetApi = new AssetApi();
            
            var createResponse = await characterApi.CreateAsync(new CharacterCreateRequest()
            {
                Payload = new CharacterCreateRequestBody()
                {
                    Assets =  new Dictionary<string, string>
                    {
                        {STYLE_ASSET_LABEL, styleId}
                    }
                }
            });
            characterId = createResponse.Data.Id;
            LoadStyleTemplate();
            CharacterObject.SetActive(false);
            await GetCachedAssets();
            var defaultAssets = await GetDefaultAssets();
            await LoadAssetPreview(defaultAssets);
            CharacterObject.SetActive(true);
        }

        private async Task<Asset[]> GetDefaultAssets()
        {
            var response = await assetApi.ListAssetsAsync(new AssetListRequest()
            {
                Params = new AssetListQueryParams()
                {
                    ExcludeTypes = "baseModel",
                    CharacterModelAssetId = styleId,
                    Limit = 100
                }
            });
            var defaultAssets = response.Data.Where( asset => asset.Name.EndsWith("_Default")).ToArray();
            return defaultAssets;
        }

        private void LoadStyleTemplate()
        {
            if(CharacterObject != null)
            {
                Destroy(CharacterObject);
                AssetMeshMap.Clear();
            }
            CharacterObject = Instantiate(GetTemplate(styleId));
            AssetsMap[STYLE_ASSET_LABEL] = new Asset {Id = styleId, Type = STYLE_ASSET_LABEL};
            var skinnedMeshes = CharacterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            AssetMeshMap[STYLE_ASSET_LABEL] = skinnedMeshes;
            OnCharacterLoaded?.Invoke(CharacterObject);
            SetupSkeletonAndAnimator();
        }

        private void SetupSkeletonAndAnimator()
        {
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
        
            var json = await File.ReadAllTextAsync(CachePaths.CACHE_ASSET_JSON_PATH);
            var cachedAssets = JsonConvert.DeserializeObject<CachedAsset[]>(json);
            var assets = cachedAssets.Select(cachedAsset =>
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
            if (asset.IsStyleAsset()) return;
            AssetsMap.Remove(asset.Type);
            
            if(AssetMeshMap.ContainsKey(string.Empty))
            {
                var assets = new Dictionary<string, string>();
                foreach (var assetInMap in AssetsMap)
                {
                    assets[assetInMap.Value.Type] = assetInMap.Value.Id;
                }
                await LoadAssetPreview(assets);
                return;
            }
            
            if(AssetMeshMap.ContainsKey(asset.Type))
            {
                foreach (var skinnedMesh in AssetMeshMap[asset.Type])
                {
                    Destroy(skinnedMesh.gameObject);
                }
                AssetMeshMap.Remove(asset.Type);
            }
        }

        public async Task LoadAssetPreview(Asset[] assets)
        {
            foreach (var asset in assets)
            {
                AssetsMap[asset.Type] = asset;
            }
            // TODO add check if asset exists in cache
            if (useCache && assets.All(asset => CanUseCache(asset.Id)))
            {
                await LoadAssetsFromCache(assets);
            }
            else {
                // add assets to map
                var assetIdMapByType = new Dictionary<string, string>();
                foreach (var assetInMap in AssetsMap)
                {
                    assetIdMapByType[assetInMap.Value.Type] = assetInMap.Value.Id;
                }
                
                await LoadAssetPreview(assetIdMapByType);
            }
            
            CharacterObject.SetActive(true);
        }
        
        public async void LoadAssetPreview(Asset asset)
        {
            if (asset.IsStyleAsset())
            {
                styleId = asset.Id;
                UpdateBaseModel();
                return;
            }
            AssetsMap[asset.Type] = asset;

            if (useCache && CanUseCache(asset.Id))
            {
                await LoadAssetFromCache(asset);
            }
            else
            {
                var assets = new Dictionary<string, string>();
                foreach (var assetInMap in AssetsMap)
                {
                    assets[assetInMap.Value.Type] = assetInMap.Value.Id;
                }
                await LoadAssetPreview(assets);
            }
        }
        private async Task LoadAssetsFromCache(Asset[] assets)
        {
            var loadedOutfits = new Dictionary<string, GameObject>();
            foreach (var asset in assets)
            {
                var gltf = new GltfImport();
                var outfit = new GameObject(asset.Id);
                outfit.transform.SetParent(loadingObject.transform);
                var path = $"{CachePaths.CACHE_ASSET_ROOT}/{styleId}/{asset.Id}";
                byte[] assetBytes = await File.ReadAllBytesAsync(path);
                await gltf.Load(assetBytes);
                await gltf.InstantiateSceneAsync(outfit.transform);
                loadedOutfits[asset.Type] = outfit;
            }

            foreach (var loadedOutfit in loadedOutfits)
            {
                var newSkinnedMeshes = loadedOutfit.Value.GetComponentsInChildren<SkinnedMeshRenderer>();
                meshTransfer.TransferMeshes(CharacterObject.transform, loadedOutfit.Value.transform, CharacterObject.transform);
                if(AssetMeshMap.ContainsKey(loadedOutfit.Key) || AssetMeshMap.ContainsKey(string.Empty))
                {
                    foreach (var skinnedMesh in AssetMeshMap[loadedOutfit.Key])
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                    AssetMeshMap.Remove(loadedOutfit.Key);
                }
                AssetMeshMap[loadedOutfit.Key] = newSkinnedMeshes;
                Destroy(loadedOutfit.Value);
            }
            
            OnCharacterLoaded?.Invoke(CharacterObject);
        }

        private async Task LoadAssetFromCache(Asset asset)
        {
            var gltf = new GltfImport();
            var outfit = new GameObject(asset.Id);
            outfit.transform.SetParent(loadingObject.transform);
            var path = $"{CachePaths.CACHE_ASSET_ROOT}/{styleId}/{asset.Id}";
            byte[] assetBytes = await File.ReadAllBytesAsync(path);
            await gltf.Load(assetBytes);
            await gltf.InstantiateSceneAsync(outfit.transform);
            var newSkinnedMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
            meshTransfer.TransferMeshes(CharacterObject.transform, outfit.transform, CharacterObject.transform);
            if(AssetMeshMap.ContainsKey(asset.Type) || AssetMeshMap.ContainsKey(string.Empty))
            {
                foreach (var skinnedMesh in AssetMeshMap[asset.Type])
                {
                    Destroy(skinnedMesh.gameObject);
                }
                AssetMeshMap.Remove(asset.Type);
            }
            AssetMeshMap[asset.Type] = newSkinnedMeshes;
            Destroy(outfit);
            OnCharacterLoaded?.Invoke(CharacterObject);
        }

        private async Task LoadAssetPreview(Dictionary<string, string> assets)
        {
            var styleIdOnStart = styleId;
            
            var gltf = new GltfImport();
            var outfit = new GameObject(characterId);
            outfit.transform.SetParent(loadingObject.transform);
            var url = characterApi.GeneratePreviewUrl(new CharacterPreviewRequest()
            {
                Id = characterId,
                Params = new CharacterPreviewQueryParams()
                {
                    Assets = assets,
                }
            });
            await gltf.Load(url);
            if (styleIdOnStart != styleId) return;
            await gltf.InstantiateSceneAsync(outfit.transform);
            if (styleIdOnStart != styleId)
            {
                Destroy(outfit);
                return;
            }
   
            var newSkinnedMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
            meshTransfer.TransferMeshes(CharacterObject.transform, outfit.transform, CharacterObject.transform);
          
            foreach (var AssetMeshMap in AssetMeshMap)
            {
                foreach (var skinnedMesh in AssetMeshMap.Value)
                {
                    Destroy(skinnedMesh.gameObject);
                }
            }
            AssetMeshMap.Clear();
            AssetMeshMap[string.Empty] = newSkinnedMeshes;
            Destroy(outfit);
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
        
        private bool CanUseCache(string assetId)
        {
            return cachedAssets != null && cachedAssets.Any(cachedAsset => cachedAsset.Id == assetId) //is asset present in cache
                   &&  !AssetMeshMap.ContainsKey(string.Empty); //is there any non cached asset loaded
        }
        
        private async void UpdateBaseModel()
        {
            var styleIdOnStart = styleId;
            var previousCharacterObject = CharacterObject;
            CharacterObject = null;
            LoadStyleTemplate();
            OnCharacterLoaded?.Invoke(previousCharacterObject);
            if (CharacterObject != null)
            {
                CharacterObject.SetActive(false);
            }
     
            var assetArray = AssetsMap.Values.ToArray();
            
            await LoadAssetPreview(assetArray); 
            var previousRotation = previousCharacterObject.transform.rotation;
            Destroy(previousCharacterObject);
            if (CharacterObject != null)
            {
                CharacterObject.transform.rotation = previousRotation;
                CharacterObject.SetActive(true);
            }
        }
    }
}
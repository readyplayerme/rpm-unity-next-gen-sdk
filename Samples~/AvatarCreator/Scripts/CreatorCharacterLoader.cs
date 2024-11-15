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
using AssetListQueryParams = ReadyPlayerMe.Api.V1.AssetListQueryParams;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class LoadedOutfit
    {
        public GameObject Outfit;
        public string Type;
        public SkinnedMeshRenderer[] Meshes;
    }
    
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
        public Dictionary<string, SkinnedMeshRenderer[]> EquippedMeshes = new Dictionary<string, SkinnedMeshRenderer[]>();
        
        [SerializeField]
        private bool useCache;
        
        Asset[] cachedAssets;
        private GameObject loadingObject;
        private bool isUpdatingBaseModel;

        private async void Start()
        {
            loadingObject = new GameObject("LoadingMeshes");
            loadingObject.SetActive(false);
            if (useCache)
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
                EquippedMeshes.Clear();
            }
            CharacterObject = Instantiate(GetTemplate(styleId));
            AssetsMap[STYLE_ASSET_LABEL] = new Asset {Id = styleId, Type = STYLE_ASSET_LABEL};
            var skinnedMeshes = CharacterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            EquippedMeshes[STYLE_ASSET_LABEL] = skinnedMeshes;
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
            
            if(EquippedMeshes.ContainsKey(string.Empty))
            {
                var assets = new Dictionary<string, string>();
                foreach (var assetInMap in AssetsMap)
                {
                    assets[assetInMap.Value.Type] = assetInMap.Value.Id;
                }
                await LoadAssetPreview(assets);
                return;
            }
            
            if(EquippedMeshes.ContainsKey(asset.Type))
            {
                foreach (var skinnedMesh in EquippedMeshes[asset.Type])
                {
                    Destroy(skinnedMesh.gameObject);
                }
                EquippedMeshes.Remove(asset.Type);
            }
        }

        public async Task LoadAssetPreview(Asset[] assets)
        {
            var loadedOutfits = new List<LoadedOutfit>();
            foreach (var asset in assets)
            {
                AssetsMap[asset.Type] = asset;
                if (useCache && CanUseCache(asset.Id))
                {
                    //await LoadAssetsFromCache(assets);
                    Debug.Log($"Loading asset of type {asset.Type} from cache");
                    var loadedOutfit = await LoadAssetFromCache(asset);
                    loadedOutfits.Add(loadedOutfit);
                }
                else
                {
                    Debug.Log($"Loading asset of type {asset.Type} from web");
                    var response = await assetApi.ListAssetsAsync(new AssetListRequest()
                    {
                        Params = new AssetListQueryParams()
                        {
                            Type = asset.Type,
                            CharacterModelAssetId = styleId,
                            Ids = new[] { asset.Id }
                        }
                    });
                    if (response.Data.Length > 0)
                    {
                        var loadedOutfit = await LoadAsset(response.Data[0]);
                        loadedOutfits.Add(loadedOutfit);
                    }
                    else
                    {
                        Debug.LogError($"Asset {asset.Id} not found");
                    }
                }
                
            }
            Debug.Log( $"Loaded {loadedOutfits.Count} outfits");
            foreach (var loadedOutfit in loadedOutfits)
            {
                meshTransfer.TransferMeshes(CharacterObject.transform, loadedOutfit.Outfit.transform, CharacterObject.transform);
                Debug.Log($"Transferred meshes for {loadedOutfit.Type}");
                if (EquippedMeshes.TryGetValue(loadedOutfit.Type, out var value))
                {
                    foreach (var skinnedMesh in value)
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                }
                EquippedMeshes[loadedOutfit.Type] = loadedOutfit.Meshes;
                Destroy(loadedOutfit.Outfit);
            }
            CharacterObject.SetActive(true);
            OnCharacterLoaded?.Invoke(CharacterObject);
            
            // // TODO add check if asset exists in cache
            // if (useCache && assets.All(asset => CanUseCache(asset.Id)))
            // {
            //     await LoadAssetsFromCache(assets);
            // }
            // else 
            // {
            //     var assetIdMapByType = new Dictionary<string, string>();
            //     foreach (var assetInMap in AssetsMap)
            //     {
            //         assetIdMapByType[assetInMap.Value.Type] = assetInMap.Value.Id;
            //     }
            //     var assetList = new List<Asset>();
            //     foreach (var assetId in assetIdMapByType)
            //     {
            //         var response = await assetApi.ListAssetsAsync(new AssetListRequest()
            //         {
            //             Params = new AssetListQueryParams()
            //             {
            //                 CharacterModelAssetId = styleId,
            //                 Ids = new[] { assetId.Value }
            //             }
            //         });
            //         assetList.AddRange(response.Data);
            //     }
            //     if( assetList.Count > 0)
            //     {
            //         //var loadedOutfits = new List<LoadedOutfit>();
            //
            //         
            //         OnCharacterLoaded?.Invoke(CharacterObject);
            //     }
            //     else
            //     {
            //         Debug.LogError($"Assets not found");
            //     }
            // }
            //
            // CharacterObject.SetActive(true);
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
                var loadedOutfit = await LoadAssetFromCache(asset);
                meshTransfer.TransferMeshes(CharacterObject.transform, loadedOutfit.Outfit.transform, CharacterObject.transform);
                foreach (var skinnedMesh in EquippedMeshes[asset.Type])
                {
                    Destroy(skinnedMesh.gameObject);
                }
                EquippedMeshes[asset.Type] = loadedOutfit.Meshes;
                Destroy(loadedOutfit.Outfit);
            }
            else
            {
                var response = await assetApi.ListAssetsAsync( new AssetListRequest()
                {
                    Params = new AssetListQueryParams()
                    {
                        Type = asset.Type,
                        CharacterModelAssetId = styleId,
                        Ids = new []{ asset.Id }
                    }
                });
                if( response.Data.Length > 0)
                {
                    var loadedOutfit = await LoadAsset(response.Data[0]);
                    meshTransfer.TransferMeshes(CharacterObject.transform, loadedOutfit.Outfit.transform, CharacterObject.transform);
                    if(EquippedMeshes.ContainsKey(asset.Type))
                    {
                        foreach (var skinnedMesh in EquippedMeshes[asset.Type])
                        {
                            Destroy(skinnedMesh.gameObject);
                        }
                        EquippedMeshes.Remove(asset.Type);
                    }
                    EquippedMeshes[asset.Type] = loadedOutfit.Meshes;
                    Destroy(loadedOutfit.Outfit);
                }
                else
                {
                    Debug.LogError($"Asset {asset.Id} not found");
                }
            }
            OnCharacterLoaded?.Invoke(CharacterObject);
        }

        private async Task<LoadedOutfit> LoadAsset(Asset asset)
        {
            var loadedOutfit = new LoadedOutfit();
            var outfit = new GameObject(asset.Id);
            outfit.transform.SetParent(loadingObject.transform);
            loadedOutfit.Type = asset.Type;
            loadedOutfit.Outfit = outfit;
            var gltf = new GltfImport();
            await gltf.Load(asset.GlbUrl);
            await gltf.InstantiateSceneAsync(outfit.transform);
            var newMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
            loadedOutfit.Meshes = newMeshes;
            return loadedOutfit;
        }
        
        private async Task<LoadedOutfit> LoadAssetFromCache(Asset asset)
        {
            var loadedOutfit = new LoadedOutfit();
            var gltf = new GltfImport();
            var outfit = new GameObject(asset.Id);
            loadedOutfit.Type = asset.Type;
            loadedOutfit.Outfit = outfit;
            outfit.transform.SetParent(loadingObject.transform);
            var path = $"{CachePaths.CACHE_ASSET_ROOT}/{styleId}/{asset.Id}";
            var assetBytes = await File.ReadAllBytesAsync(path);
            await gltf.Load(assetBytes);
            await gltf.InstantiateSceneAsync(outfit.transform);
            var newSkinnedMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
            loadedOutfit.Meshes = newSkinnedMeshes;
            return loadedOutfit;
            // meshTransfer.TransferMeshes(CharacterObject.transform, outfit.transform, CharacterObject.transform);
            // if(EquippedMeshes.ContainsKey(asset.Type) || EquippedMeshes.ContainsKey(string.Empty))
            // {
            //     foreach (var skinnedMesh in EquippedMeshes[asset.Type])
            //     {
            //         Destroy(skinnedMesh.gameObject);
            //     }
            //     EquippedMeshes.Remove(asset.Type);
            // }
            // EquippedMeshes[asset.Type] = newSkinnedMeshes;
            // Destroy(outfit);
            // OnCharacterLoaded?.Invoke(CharacterObject);
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
                if(EquippedMeshes.ContainsKey(loadedOutfit.Key) || EquippedMeshes.ContainsKey(string.Empty))
                {
                    foreach (var skinnedMesh in EquippedMeshes[loadedOutfit.Key])
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                    EquippedMeshes.Remove(loadedOutfit.Key);
                }
                EquippedMeshes[loadedOutfit.Key] = newSkinnedMeshes;
                Destroy(loadedOutfit.Value);
            }
            
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
          
            foreach (var AssetMeshMap in EquippedMeshes)
            {
                foreach (var skinnedMesh in AssetMeshMap.Value)
                {
                    Destroy(skinnedMesh.gameObject);
                }
            }
            EquippedMeshes.Clear();
            EquippedMeshes[string.Empty] = newSkinnedMeshes;
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
                   &&  !EquippedMeshes.ContainsKey(string.Empty); //is there any non cached asset loaded
        }
        
        private async void UpdateBaseModel()
        {
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
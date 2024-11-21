using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private CharacterStyleTemplateConfig characterStyleTemplateConfig;
        
        [SerializeField]
        private string styleId = "665e05e758e847063761c985";
        
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
        private bool useCache;
        
        Asset[] cachedAssets;
        private GameObject loadingObject;
        private bool isUpdatingBaseModel;
        
        private CancellationTokenSource cancellationTokenSource;
        
        private const string STORED_CHARACTER_PREF = "RPM_Creator_Character";
        private const string STORED_CHARACTER_STYLE_PREF = "RPM_Creator_Character_STYLE";

        private async void Start()
        {
            loadingObject = new GameObject("LoadingMeshes");
            loadingObject.SetActive(false);
            
            //disable if cache doesn't exist
            useCache = File.Exists(CachePaths.PROJECT_CACHE_ASSET_ZIP_PATH);
            if (useCache)
            {
                var cacheGenerator = new CacheGenerator();
                cacheGenerator.ExtractCache();
            }
            //disable if cache doesn't exist
            useCache = File.Exists(CachePaths.CACHE_ASSET_JSON_PATH);
            characterApi = new CharacterApi();
            meshTransfer = new MeshTransfer();
            skeletonBuilder = new SkeletonBuilder();
            assetApi = new AssetApi();
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var createResponse = await characterApi.CreateAsync(new CharacterCreateRequest()
                {
                    Payload = new CharacterCreateRequestBody()
                    {
                        Assets =  new Dictionary<string, string>
                        {
                            {Constants.STYLE_ASSET_LABEL, styleId}
                        }
                    }
                }, cancellationTokenSource.Token);
                characterId = createResponse.Data.Id;
                PlayerPrefs.SetString(STORED_CHARACTER_PREF, characterId);
                PlayerPrefs.SetString(STORED_CHARACTER_STYLE_PREF, styleId);
                LoadStyleTemplate();
                CharacterObject.SetActive(false);
                await GetCachedAssets();
                var defaultAssets = await GetDefaultAssets();
                await LoadAssetPreview(defaultAssets);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            CharacterObject.SetActive(true);
        }

        private void OnDestroy()
        {
            Destroy(CharacterObject);
            Destroy(loadingObject);
            cancellationTokenSource?.Cancel();
        }

        private async Task<Asset[]> GetDefaultAssets()
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var response = await assetApi.ListAssetsAsync(new AssetListRequest()
                {
                    Params = new AssetListQueryParams()
                    {
                        ExcludeTypes = "baseModel",
                        CharacterModelAssetId = styleId,
                        Limit = 100
                    }
                }, cancellationTokenSource.Token);
                // TODO using _Default in name is a temp solution
                var defaultAssets = response.Data.Where( asset => asset.Name.EndsWith("_Default")).ToArray();
                return defaultAssets;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }

        private void LoadStyleTemplate()
        {
            if(CharacterObject != null)
            {
                Destroy(CharacterObject);
                EquippedMeshes.Clear();
            }
            CharacterObject = Instantiate(characterStyleTemplateConfig.GetTemplate( styleId, "Creator"));
            AssetsMap[Constants.STYLE_ASSET_LABEL] = new Asset {Id = styleId, Type = Constants.STYLE_ASSET_LABEL};
            var skinnedMeshes = CharacterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            EquippedMeshes[Constants.STYLE_ASSET_LABEL] = skinnedMeshes;
            OnCharacterLoaded?.Invoke(CharacterObject);
            SetupSkeletonAndAnimator();
        }

        private void SetupSkeletonAndAnimator()
        {
            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(Constants.SKELETON_DEFINITION_LABEL)
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

            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var json = await File.ReadAllTextAsync(CachePaths.CACHE_ASSET_JSON_PATH, cancellationTokenSource.Token);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            
            return null;
        }

        public void RemoveAsset(Asset asset)
        {
            if (asset.IsStyleAsset()) return;
            AssetsMap.Remove(asset.Type);
            
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
                    var loadedOutfit = await LoadAssetFromCache(asset);
                    loadedOutfits.Add(loadedOutfit);
                }
            }
            var nonCachedAssets = assets.Where(asset => !CanUseCache(asset.Id)).ToArray();
            if (nonCachedAssets.Length > 0)
            {
                var nonCachedAssetIds = nonCachedAssets.Select(asset => asset.Id).ToArray();
                var startTime = Time.realtimeSinceStartup;
                var response = await assetApi.ListAssetsAsync(new AssetListRequest()
                {
                    Params = new AssetListQueryParams()
                    {
                        CharacterModelAssetId = styleId,
                        Ids = nonCachedAssetIds
                    }
                });
                Debug.Log($"Time taken to fetch list of assets: {Time.realtimeSinceStartup - startTime}");
                if (response.Data.Length > 0)
                {
                    foreach (var refittedAssets in response.Data)
                    {
                        var loadedOutfit = await LoadAsset(refittedAssets);
                        loadedOutfits.Add(loadedOutfit);
                    }
                }
                else
                {
                    Debug.LogError($"Assets not found");
                }
            }
            foreach (var loadedOutfit in loadedOutfits)
            {
                meshTransfer.TransferMeshes(CharacterObject.transform, loadedOutfit.Outfit.transform, CharacterObject.transform);
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
            ApplyUpdates();

        }
        
        public async void ApplyUpdates()
        {
            var idByType = new Dictionary<string, string>();
            foreach (var asset in AssetsMap)
            {
                idByType[asset.Value.Type] = asset.Value.Id;
            }
            try
            {
                var response = await characterApi.UpdateAsync( new CharacterUpdateRequest()
                {
                    Id = characterId,
                    Payload = new CharacterUpdateRequestBody()
                    {
                        Assets = idByType
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public void UpdateCharacter()
        {
            Debug.Log($" applying assets to character: {AssetsMap.Count}");
            // print asset types in AssetsMap
            foreach (var asset in AssetsMap)
            {
                Debug.Log(asset.Value.Type);
            }
            var response = characterApi.UpdateAsync( new CharacterUpdateRequest()
            {
                Id = characterId,
                Payload = new CharacterUpdateRequestBody()
                {
                    Assets = AssetsMap.ToDictionary(asset => asset.Value.Type, asset => asset.Value.Id)
                }
            });
            Debug.Log($" Character updated: {response}");
        }
        
        public async void LoadAssetPreview(Asset asset)
        {
            if (asset.IsStyleAsset())
            {
                styleId = asset.Id;
                UpdateBaseModel();
                PlayerPrefs.SetString(STORED_CHARACTER_STYLE_PREF, styleId);
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
                var startTime = Time.realtimeSinceStartup;
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    var response = await assetApi.ListAssetsAsync( new AssetListRequest()
                    {
                        Params = new AssetListQueryParams()
                        {
                            Type = asset.Type,
                            CharacterModelAssetId = styleId,
                            Ids = new []{ asset.Id }
                        }
                    }, cancellationTokenSource.Token);
                    Debug.Log($"Time taken to fetch list of assets: {Time.realtimeSinceStartup - startTime}");
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            ApplyUpdates();
            OnCharacterLoaded?.Invoke(CharacterObject);
        }

        private async Task<LoadedOutfit> LoadAsset(Asset asset)
        {
            cancellationTokenSource = new CancellationTokenSource();
            
            var loadedOutfit = new LoadedOutfit();
            var outfit = new GameObject(asset.Id);
            outfit.transform.SetParent(loadingObject.transform);
            loadedOutfit.Type = asset.Type;
            loadedOutfit.Outfit = outfit;
            var gltf = new GltfImport();
            try
            {
                await gltf.Load(asset.GlbUrl, null, cancellationTokenSource.Token);
                await gltf.InstantiateSceneAsync(outfit.transform, 0 , cancellationTokenSource.Token);
                var newMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
                loadedOutfit.Meshes = newMeshes;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Asset loading was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load asset: {ex.Message}");
                throw;
            }

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
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                var assetBytes = await File.ReadAllBytesAsync(path, token);
                await gltf.Load(assetBytes, null, null, token);
                await gltf.InstantiateSceneAsync(outfit.transform, 0, token);
                var newSkinnedMeshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
                loadedOutfit.Meshes = newSkinnedMeshes;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Asset loading was canceled.");
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load asset: {ex.Message}");
                throw; // Re-throw to propagate the error if necessary
            }

            return loadedOutfit;
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
        
        private bool CanUseCache(string assetId)
        {
            return cachedAssets != null && cachedAssets.Any(cachedAsset => cachedAsset.Id == assetId); //is asset present in cache
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
            Debug.Log($"number of assets: {AssetsMap.Values.Count}");
            var assetArray = AssetsMap.Values.Where(asset => !asset.IsStyleAsset()).ToArray(); // asset without style model
            Debug.Log($"number of assets after: {assetArray.Length}");
            // print asset array types
            foreach (var asset in assetArray)
            {
                Debug.Log(asset.Type);
            }
            await LoadAssetPreview(assetArray); 
            PlayerPrefs.SetString(STORED_CHARACTER_STYLE_PREF, styleId);
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
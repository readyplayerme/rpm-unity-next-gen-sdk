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
        private CharacterBlueprintTemplateConfig characterBlueprintTemplateConfig;
        
        [SerializeField]
        private string blueprintId = "665e05e758e847063761c985";
        
        private CharacterApi characterApi;
        private MeshTransfer meshTransfer;
        private SkeletonBuilder skeletonBuilder;
        
        private AssetLoader assetLoader;
        private string characterId;
        private CharacterData characterData;
        private GameObject characterObject;
        
        public UnityEvent<GameObject> OnCharacterLoaded;
        
        private AssetApi assetApi;
        
        private Dictionary<string, Asset> assetsMap = new Dictionary<string, Asset>();
        private Dictionary<string, SkinnedMeshRenderer[]> equippedMeshes = new Dictionary<string, SkinnedMeshRenderer[]>();
        private bool useCache;
        
        Asset[] assetsInCache;
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
                            {Constants.STYLE_ASSET_LABEL, blueprintId}
                        }
                    }
                }, cancellationTokenSource.Token);
                characterId = createResponse.Data.Id;
                PlayerPrefs.SetString(STORED_CHARACTER_PREF, characterId);
                PlayerPrefs.SetString(STORED_CHARACTER_STYLE_PREF, blueprintId);
                LoadStyleTemplate();
                characterObject.SetActive(false);
                await GetCachedAssets();
                await LoadDefaultAssets();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            characterObject.SetActive(true);
        }

        private async Task LoadDefaultAssets()
        {
            var defaultAssets = await GetDefaultAssets();
            await LoadAssetPreview(defaultAssets);
        }

        private void OnDestroy()
        {
            Destroy(characterObject);
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
                        CharacterModelAssetId = blueprintId,
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
            if(characterObject != null)
            {
                Destroy(characterObject);
                equippedMeshes.Clear();
            }
            characterObject = Instantiate(characterBlueprintTemplateConfig.GetTemplate( blueprintId, "Creator"));
            assetsMap[Constants.STYLE_ASSET_LABEL] = new Asset {Id = blueprintId, Type = Constants.STYLE_ASSET_LABEL};
            var skinnedMeshes = characterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            equippedMeshes[Constants.STYLE_ASSET_LABEL] = skinnedMeshes;
            OnCharacterLoaded?.Invoke(characterObject);
            SetupSkeletonAndAnimator();
        }

        private void SetupSkeletonAndAnimator()
        {
            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(Constants.SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == blueprintId)?
                .definition;

            characterObject.gameObject.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;
            
            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                skeletonBuilder.Build(characterObject.gameObject, skeletonDefinition != null
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
                
                assetsInCache = await LoadAssetsFromCacheAsync(blueprintId);
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
                var cachedAssetArray = JsonConvert.DeserializeObject<CachedAsset[]>(json);
                var assets = cachedAssetArray.Select(cachedAsset =>
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
            assetsMap.Remove(asset.Type);
            
            if(equippedMeshes.ContainsKey(asset.Type))
            {
                foreach (var skinnedMesh in equippedMeshes[asset.Type])
                {
                    Destroy(skinnedMesh.gameObject);
                }
                equippedMeshes.Remove(asset.Type);
            }
        }

        public async Task LoadAssetPreview(Asset[] assets)
        {
            var loadedOutfits = new List<LoadedOutfit>();
            var nonCachedAssetList = new List<Asset>();
            foreach (var asset in assets)
            {
                assetsMap[asset.Type] = asset;
                if (useCache && CanUseCache(asset.Id))
                {
                    var loadedOutfit = await LoadAssetFromCache(asset);
                    loadedOutfits.Add(loadedOutfit);
                }
                else
                {
                    nonCachedAssetList.Add(asset);
                }
            }
      
            if (nonCachedAssetList.Count > 0)
            {
                var nonCachedAssetIds = nonCachedAssetList.Select(asset => asset.Id).ToArray();
                var startTime = Time.realtimeSinceStartup;
                var response = await assetApi.ListAssetsAsync(new AssetListRequest()
                {
                    Params = new AssetListQueryParams()
                    {
                        CharacterModelAssetId = blueprintId,
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
            TransferLoadedOutfits(loadedOutfits);
            characterObject.SetActive(true);
            OnCharacterLoaded?.Invoke(characterObject);
            ApplyUpdates();

        }

        private void TransferLoadedOutfits(List<LoadedOutfit> loadedOutfits)
        {
            foreach (var loadedOutfit in loadedOutfits)
            {
                meshTransfer.TransferMeshes(characterObject.transform, loadedOutfit.Outfit.transform, characterObject.transform);
                if (equippedMeshes.TryGetValue(loadedOutfit.Type, out var value))
                {
                    foreach (var skinnedMesh in value)
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                }
                equippedMeshes[loadedOutfit.Type] = loadedOutfit.Meshes;
                Destroy(loadedOutfit.Outfit);
            }
        }

        public async void ApplyUpdates()
        {
            var idByType = new Dictionary<string, string>();
            foreach (var asset in assetsMap)
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
            foreach (var asset in assetsMap)
            {
                Debug.Log(asset.Value.Type);
            }
            var response = characterApi.UpdateAsync( new CharacterUpdateRequest()
            {
                Id = characterId,
                Payload = new CharacterUpdateRequestBody()
                {
                    Assets = assetsMap.ToDictionary(asset => asset.Value.Type, asset => asset.Value.Id)
                }
            });
        }
        
        public async void LoadAssetPreview(Asset asset)
        {
            if (asset.IsStyleAsset())
            {
                blueprintId = asset.Id;
                UpdateBaseModel();
                PlayerPrefs.SetString(STORED_CHARACTER_STYLE_PREF, blueprintId);
                return;
            }
            assetsMap[asset.Type] = asset;

            
            if (useCache && CanUseCache(asset.Id))
            {
                var loadedOutfit = await LoadAssetFromCache(asset);
                meshTransfer.TransferMeshes(characterObject.transform, loadedOutfit.Outfit.transform, characterObject.transform);
                if(equippedMeshes.ContainsKey(asset.Type))
                {
                    foreach (var skinnedMesh in equippedMeshes[asset.Type])
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                    equippedMeshes.Remove(asset.Type);
                }
                equippedMeshes[asset.Type] = loadedOutfit.Meshes;
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
                            CharacterModelAssetId = blueprintId,
                            Ids = new []{ asset.Id }
                        }
                    }, cancellationTokenSource.Token);
                    Debug.Log($"Time taken to fetch list of assets: {Time.realtimeSinceStartup - startTime}");
                    if( response.Data.Length > 0)
                    {
                        var loadedOutfit = await LoadAsset(response.Data[0]);
                        meshTransfer.TransferMeshes(characterObject.transform, loadedOutfit.Outfit.transform, characterObject.transform);
                        if(equippedMeshes.ContainsKey(asset.Type))
                        {
                            foreach (var skinnedMesh in equippedMeshes[asset.Type])
                            {
                                Destroy(skinnedMesh.gameObject);
                            }
                            equippedMeshes.Remove(asset.Type);
                        }
                        equippedMeshes[asset.Type] = loadedOutfit.Meshes;
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
            OnCharacterLoaded?.Invoke(characterObject);
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

            var path = $"{CachePaths.CACHE_ASSET_ROOT}/{blueprintId}/{asset.Id}";
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
                var path = $"{CachePaths.CACHE_ASSET_ROOT}/{blueprintId}/{asset.Id}";
                byte[] assetBytes = await File.ReadAllBytesAsync(path);
                await gltf.Load(assetBytes);
                await gltf.InstantiateSceneAsync(outfit.transform);
                loadedOutfits[asset.Type] = outfit;
            }

            foreach (var loadedOutfit in loadedOutfits)
            {
                var newSkinnedMeshes = loadedOutfit.Value.GetComponentsInChildren<SkinnedMeshRenderer>();
                meshTransfer.TransferMeshes(characterObject.transform, loadedOutfit.Value.transform, characterObject.transform);
                if(equippedMeshes.ContainsKey(loadedOutfit.Key) || equippedMeshes.ContainsKey(string.Empty))
                {
                    foreach (var skinnedMesh in equippedMeshes[loadedOutfit.Key])
                    {
                        Destroy(skinnedMesh.gameObject);
                    }
                    equippedMeshes.Remove(loadedOutfit.Key);
                }
                equippedMeshes[loadedOutfit.Key] = newSkinnedMeshes;
                Destroy(loadedOutfit.Value);
            }
            
            OnCharacterLoaded?.Invoke(characterObject);
        }
        
        private bool CanUseCache(string assetId)
        {
            return assetsInCache != null && assetsInCache.Any(cachedAsset => cachedAsset.Id == assetId); //is asset present in cache
        }
        
        private async void UpdateBaseModel()
        {
            var previousCharacterObject = characterObject;
            characterObject = null;
            LoadStyleTemplate();
            OnCharacterLoaded?.Invoke(previousCharacterObject);
            if (characterObject != null)
            {
                characterObject.SetActive(false);
            }
            Debug.Log($"number of assets: {assetsMap.Values.Count}");
            var assetArray = assetsMap.Values.Where(asset => !asset.IsStyleAsset()).ToArray(); // asset without style model
            Debug.Log($"number of assets after: {assetArray.Length}");
            // print asset array types
            foreach (var asset in assetArray)
            {
                Debug.Log(asset.Type);
            }
            await LoadAssetPreview(assetArray); 
            PlayerPrefs.SetString(STORED_CHARACTER_STYLE_PREF, blueprintId);
            var previousRotation = previousCharacterObject.transform.rotation;
            Destroy(previousCharacterObject);
            if (characterObject != null)
            {
                characterObject.transform.rotation = previousRotation;
                characterObject.SetActive(true);
            }
        }
    }
}
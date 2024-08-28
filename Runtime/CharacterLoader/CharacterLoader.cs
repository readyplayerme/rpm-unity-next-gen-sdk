using GLTFast;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    public class CharacterLoader
    {
        private readonly CharacterApi _characterApi;
        private readonly MeshTransfer _meshTransfer;
        private readonly SkeletonBuilder _skeletonBuilder;

        public CharacterLoader()
        {
            _characterApi = new CharacterApi();
            _meshTransfer = new MeshTransfer();
            _skeletonBuilder = new SkeletonBuilder();
        }

        public virtual Task<CharacterData> PreviewAsync(
            string id,
            Dictionary<string, string> assets,
            string templateTagOrId = null,
            CancellationToken cancellationToken = default
        )
        {
            var template = GetTemplate(templateTagOrId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;
            templateInstance?.SetActive(false);

            return PreviewAsync(id, assets, templateInstance, cancellationToken);
        }

        public virtual async Task<CharacterData> PreviewAsync(
            string id,
            Dictionary<string, string> assets,
            GameObject template = null,
            CancellationToken cancellationToken = default
        )
        {
            assets.TryGetValue("baseModel", out var styleId);

            if (string.IsNullOrEmpty(styleId))
            {
                var characterResponse = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
                {
                    Id = id,
                });

                styleId = characterResponse.Data.Assets["baseModel"];
            }

            var previewUrl = _characterApi.GeneratePreviewUrl(new CharacterPreviewRequest()
            {
                Id = id,
                Params = new CharacterPreviewQueryParams()
                {
                    Assets = assets
                }
            });

            return await LoadAsync(id, styleId, previewUrl, template, cancellationToken);
        }

        public virtual async Task<CharacterData> LoadAsync(string id)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = id,
            });

            return await LoadAsync(
                response.Data.Id,
                response.Data.Assets["baseModel"],
                response.Data.GlbUrl,
                null
            );
        }

        public virtual async Task<CharacterData> LoadAsync(string id, string templateTagOrId, CancellationToken cancellationToken = default)
        {
            var response = await _characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = id,
            });

            var template = GetTemplate(templateTagOrId);
            var templateInstance = template != null ? Object.Instantiate(template) : null;
            templateInstance?.SetActive(false);

            return await LoadAsync(
                response.Data.Id,
                response.Data.Assets["baseModel"],
                response.Data.GlbUrl,
                templateInstance,
                cancellationToken
            );
        }

        public virtual async Task<CharacterData> LoadFromCacheAsync(CharacterData original, Asset asset, string templateId)
        {
            byte[] assetBytes = await GetAssetBytesFromCache(asset, templateId);
            
            var gltf = new GltfImport();
            await gltf.Load(assetBytes);
        
            var outfit = new GameObject("outfit");
            await gltf.InstantiateSceneAsync(outfit.transform);
            
            SwapAsset(original, asset, outfit);
            
            return original;
        }

        public virtual async Task<CharacterData> LoadAsync(
            string id,
            string styleId,
            string loadFrom,
            GameObject template,
            CancellationToken cancellationToken = default
        )
        {
            var gltf = new GltfImport();

            if (!await gltf.Load(loadFrom, null, cancellationToken))
                return null;

            if (cancellationToken.IsCancellationRequested)
            {
                gltf.Dispose();
                Object.Destroy(template);
                return null;
            }

            var character = new GameObject(id);

            await gltf.InstantiateSceneAsync(character.transform);

            if (template == null)
                return InitCharacter(character, id, styleId);

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>("SkeletonDefinitionConfig")
                .definitionLinks
                .FirstOrDefault(p => p.characterStyleId == styleId)?
                .definition;

            // Update skeleton and transfer mesh
            template.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;
            
            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                _skeletonBuilder.Build(template, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }

            _meshTransfer.Transfer(character, template);
            
            animator.enabled = true;

            return InitCharacter(template, id, styleId);
        }
        
        protected virtual GameObject GetTemplate(string templateTagOrId)
        {
            if (string.IsNullOrEmpty(templateTagOrId))
                return null;

            return Resources
                .Load<CharacterStyleTemplateConfig>($"CharacterStyleTemplateConfig")?
                .templates.FirstOrDefault(p => p.id == templateTagOrId || p.tags.Contains(templateTagOrId))?
                .template;
        }

        protected virtual CharacterData InitCharacter(GameObject character, string id, string styleId)
        {
            var data = character.GetComponent<CharacterData>();

            if (data == null)
                data = character.AddComponent<CharacterData>();
            
            character.SetActive(true);

            return data.Initialize(id, styleId);
        }
        
        private async Task<byte[]> GetAssetBytesFromCache(Asset asset, string templateId)
        {
            string folderPath = $"{Application.persistentDataPath}/Local Cache/Assets/{templateId}";
            string filePath = $"{folderPath}/{asset.Id}";
            byte[] assetBytes;

            // Check if the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        
            // Check if the file exists in the cache and return the bytes if it does
            if(File.Exists(filePath))
            {
                assetBytes = await File.ReadAllBytesAsync(filePath);
            }
            // If not, download the asset and save it to the cache and return the bytes
            else
            {
                using UnityWebRequest request = UnityWebRequest.Get(asset.GlbUrl);
                AsyncOperation op = request.SendWebRequest();
            
                while (!op.isDone)
                {
                    await Task.Yield();
                }
            
                if (request.result == UnityWebRequest.Result.Success)
                {
                    assetBytes = request.downloadHandler.data;
                    await File.WriteAllBytesAsync(filePath, assetBytes);
                }
                else
                {
                    Debug.LogError(request.error);
                    return null;
                }
            }
        
            return assetBytes;
        }
        
        private void SwapAsset(CharacterData original, Asset asset, GameObject outfit)
        {
            // TODO: Add handing baseMesh swap case, where all assets must be gone
            if(original.AssetMeshes.ContainsKey(asset.Type))
            {
                foreach (var mesh in original.AssetMeshes[asset.Type])
                {
                    if(mesh != null)
                        Object.Destroy(mesh.gameObject);
                }
            
                original.AssetMeshes.Remove(asset.Type);
            }

            var meshes = outfit.GetComponentsInChildren<SkinnedMeshRenderer>();
            original.AssetMeshes.Add(asset.Type, meshes);
        
            var bones = GetBones(original.transform);
            TransferMeshes(original.transform, outfit.transform, original.transform, bones);
            Object.Destroy(outfit);
        }
            
        private void TransferMeshes(Transform targetArmature, Transform sourceArmature, Transform rootBone, Transform[] bones)
        {
            Renderer[] sourceRenderers = sourceArmature.GetComponentsInChildren<Renderer>();
        
            foreach (Renderer renderer in sourceRenderers)
            {
                Transform[] bonesCopy = new Transform[bones.Length];
                Transform[] sourceBones = GetBones(sourceArmature);
            
                for (int i = 0; i < bones.Length; i++)
                {
                    for(int j = 0; j < bones.Length; j++)
                    {
                        if(bones.Length <= j)
                            continue;

                        if (sourceBones.Length <= i)
                            continue;
                    
                        if (bones[j].name == sourceBones[i].name)
                        {
                            bonesCopy[i] = bones[j];
                            break;
                        }
                    }
                }
            
                renderer.gameObject.transform.SetParent(targetArmature);
                renderer.gameObject.transform.localPosition = Vector3.zero;
                renderer.gameObject.transform.localEulerAngles = Vector3.zero;

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.rootBone = rootBone;
                    skinnedMeshRenderer.bones = bonesCopy;
                
                    skinnedMeshRenderer.sharedMesh.RecalculateBounds();
                }
            }

            if (rootBone != null)
                rootBone.SetAsLastSibling();
        }

        private Transform[] GetBones(Transform targetArmature)
        {
            SkinnedMeshRenderer sampleMesh = targetArmature.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
            Transform[] bones = sampleMesh.bones;
            return bones;
        }
    }
}

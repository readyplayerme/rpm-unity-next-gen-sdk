using System;
using System.Linq;
using System.Threading;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class PlayerCharacterLoader : MonoBehaviour
    {
        [SerializeField]
        private CharacterTemplateConfig characterTemplateConfig;
        [SerializeField]
        private Camera thirdPersonCamera;
        private GameObject character;
        private CharacterApi characterApi;
        private MeshTransfer meshTransfer;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public UnityEvent<GameObject> OnCharacterLoaded;

        private async void Start()
        {
            if (character != null)
            {
                Destroy(character);
            }
            characterApi = new CharacterApi();
            meshTransfer = new MeshTransfer();
            var characterId = PlayerPrefs.GetString(CreatorConstants.STORED_CHARACTER_PREF);
            var blueprintId = PlayerPrefs.GetString(CreatorConstants.STORED_CHARACTER_BLUEPRINT_PREF);
            var findCharacterResponse = await characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = characterId
            });
            character = Instantiate(characterTemplateConfig.GetTemplatePrefab(blueprintId, "CreatorPlayer"));
            var playerController = character.GetComponent<ThirdPersonMovement>();
            if(playerController == null)
            {
                playerController.SetCamera(thirdPersonCamera.transform);
                return;
            }
            character.SetActive(false);
            try
            {
                var outfit = new GameObject(characterId);
                var gltf = new GltfImport();
                cancellationTokenSource = new CancellationTokenSource();
                await gltf.Load(findCharacterResponse.Data.GlbUrl, null, cancellationTokenSource.Token);
                await gltf.InstantiateSceneAsync(outfit.transform, 0, cancellationTokenSource.Token);
                meshTransfer.TransferMeshes(character.transform, outfit.transform, character.transform);
                Destroy(outfit);
                SetupSkeletonAndAnimator(blueprintId);
                character.SetActive(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            OnCharacterLoaded.Invoke(character);

        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
        }

        private void SetupSkeletonAndAnimator(string blueprintId)
        {

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(CreatorConstants.SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterBlueprintId == blueprintId)?
                .definition;

            character.gameObject.TryGetComponent<Animator>(out var animator);
            animator.enabled = false;

            var animationAvatar = animator.avatar;
            if (animationAvatar == null)
            {
                var skeletonBuilder = new SkeletonBuilder();
                skeletonBuilder.Build(character.gameObject, skeletonDefinition != null
                    ? skeletonDefinition.GetHumanBones()
                    : null
                );
            }

            animator.enabled = true;
        }
        
        public void SetInputEnabled(bool isEnabled)
        {
            var thirdPersonController = character.GetComponent<ThirdPersonController>();
            if(thirdPersonController == null)
            {
                return;
            }
            thirdPersonController.SetInputEnabled(isEnabled);
        }
    }
}

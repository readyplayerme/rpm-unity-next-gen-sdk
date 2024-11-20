using System;
using System.Linq;
using System.Threading;
using GLTFast;
using ReadyPlayerMe.Api.V1;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class PlayerCharacterLoader : MonoBehaviour
    {
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
            var characterId = PlayerPrefs.GetString(Constants.STORED_CHARACTER_PREF);
            var styleId = PlayerPrefs.GetString(Constants.STORED_CHARACTER_STYLE_PREF);
            var findCharacterResponse = await characterApi.FindByIdAsync(new CharacterFindByIdRequest()
            {
                Id = characterId
            });
            character = Instantiate(GetTemplate(styleId));
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
                SetupSkeletonAndAnimator(styleId);
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

        protected virtual GameObject GetTemplate(string templateTagOrId)
        {
            if (string.IsNullOrEmpty(templateTagOrId))
                return null;
            var templates = Resources.Load<CharacterStyleTemplateConfig>(Constants.CHARACTER_STYLE_TEMPLATE_LABEL).templates.Where( p => p.id.Contains(templateTagOrId));
            var characterStyleTemplates = templates as CharacterStyleTemplate[] ?? templates.ToArray();
            Debug.Log( $"   number of templates found {characterStyleTemplates.Count()}");
            var template = characterStyleTemplates.FirstOrDefault(p =>  p.tags.Contains("Player"));
            return template?.template;
        }

        private void SetupSkeletonAndAnimator(string styleId)
        {

            var skeletonDefinition = Resources.Load<SkeletonDefinitionConfig>(Constants.SKELETON_DEFINITION_LABEL)
                .definitionLinks
                .FirstOrDefault(p => p.characterStyleId == styleId)?
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
    }
}

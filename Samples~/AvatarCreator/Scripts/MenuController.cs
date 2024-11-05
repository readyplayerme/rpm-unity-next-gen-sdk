using UnityEngine;
using System.Linq;
using System.Threading;
using System.Collections;
using ReadyPlayerMe.Data;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Demo
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private CategoryController categoryController;
        [SerializeField] private AssetPageController assetPageController;
        [SerializeField] private AssetCard assetCard;
        [SerializeField] private EquipButton equipButton;
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private Animation menuAnimation;
        [SerializeField] private DragRotate dragRotate;
        
        private CharacterApi characterApi;
        private string selectedCategory;
        private string characterId;
        private string baseModelId;
        
        private CharacterData character;
        private CharacterData previewCharacter;
        private Vector3 previewCharacterPosition = new Vector3(0, 3, 0);
        
        private Asset previousAsset;
        private CancellationTokenSource cts = new CancellationTokenSource();
        
        private AssetLoader assetLoader;
        private CharacterLoader characterLoader;
        
        private void Start()
        {
            assetLoader = new AssetLoader();
            characterLoader = new CharacterLoader();
            
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
            EventAggregator.Instance.OnCategoriesLoaded += () => categoryController.SelectFirstCategory();
            EventAggregator.Instance.OnAssetSelected += OnAssetSelected;
            EventAggregator.Instance.OnPageChanged += OnPageChanged;
            EventAggregator.Instance.OnAssetEquipped += OnAssetEquipped;
            EventAggregator.Instance.OnAssetUnequipped += OnAssetUnequipped;
            categoryController.LoadCategories();
            loadingCanvas.SetActive(true);
            
            LoadCharacter();
        }

        private void OnCategorySelected(string category)
        {
            selectedCategory = category;
            assetPageController.LoadAssets(category);
        }
        
        private void OnAssetSelected(Asset asset)
        {
            assetCard.Initialize(asset);
            equipButton.Loading(asset);
            EquipAsset(asset);
        }
        
        private void OnPageChanged(int page)
        {
            assetPageController.LoadAssets(selectedCategory, page);
        }

        private async void LoadCharacter()
        {
            if (PlayerPrefs.HasKey("CharacterId"))
            {
                characterId = PlayerPrefs.GetString("CharacterId");
                baseModelId = PlayerPrefs.GetString("BaseModelId");
                
                character = await characterLoader.LoadAsync(characterId, baseModelId, new Asset()
                {
                    Type = "baseModel",
                    Id = baseModelId
                });
                character.transform.position = new Vector3(0, 0, -0.5f);

                dragRotate.Target = character.transform;
                StartCoroutine(HideLoadingCanvas());
            }
            else
            {
                characterApi = new CharacterApi();
                var response = await characterApi.CreateAsync(new CharacterCreateRequest());

                characterId = response.Data.Id;
                PlayerPrefs.SetString("CharacterId", characterId);
                
                Debug.Log("Character ID: " + characterId);
                assetPageController.EquipedAssets.TryGetValue("baseModel", out string currentBaseModelId);
                PlayerPrefs.SetString("BaseModelId", currentBaseModelId);
                
                character = await characterLoader.LoadCharacter(currentBaseModelId);
                character.transform.position = new Vector3(0, 0, -0.5f);

                dragRotate.Target = character.transform;
                StartCoroutine(HideLoadingCanvas());
            }
        }
        
        private async void EquipAsset(Asset asset)
        {
            equipButton.gameObject.SetActive(true);
            equipButton.Loading(asset);
            
            previousAsset = asset;
            
            if (previewCharacter)
            {
                Destroy(previewCharacter.gameObject);
            }
            
            assetPageController.EquipedAssets.TryGetValue("baseModel", out string currentBaseModelId);
            PlayerPrefs.SetString("BaseModelId", currentBaseModelId);
            
            var preview = await characterLoader.LoadAsync(characterId, currentBaseModelId, asset);
            
            if(preview == null) return;
            
            previewCharacter = preview;
            
            previewCharacter.transform.position = previewCharacterPosition;
            previewCharacter.name = "[Preview Character]";
            
            equipButton.ToggleEquipState(true);
        }
        
        private void OnAssetEquipped(Asset asset)
        {
            SwapCharacters(ref character, ref previewCharacter);
            
            Animator animator = character.GetComponent<Animator>();
            string[] lowerCategories = {"bottom", "footwear"};
            
            if(lowerCategories.Contains(previousAsset.Type))
            {
                animator.SetTrigger("Look Lower");
            }
            else
            {
                animator.SetTrigger("Look Upper");
            }
        }

        private void OnAssetUnequipped(Asset asset)
        {
            SwapCharacters(ref character, ref previewCharacter);
            equipButton.ToggleEquipState(true);
        }
        
        private IEnumerator HideLoadingCanvas()
        {
            Animation loadingCanvasAnimation = loadingCanvas.GetComponent<Animation>();
            loadingCanvasAnimation.Play("bounce_and_fade_out");
            
            yield return new WaitForSeconds(0.5f);
            
            loadingCanvas.SetActive(false);
            
            menuAnimation.Play("left_panel_fade_in");
        }
        
        private void SwapCharacters(ref CharacterData first, ref CharacterData second)
        {
            (first, second) = (second, first);
            
            first.transform.position = second.transform.position;
            first.transform.rotation = second.transform.rotation;
            
            Animator firstAnimator = first.GetComponent<Animator>();
            Animator secondAnimator = second.GetComponent<Animator>();
            secondAnimator.Play(0, -1, firstAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            
            first.gameObject.name = "[Current Character]";
            second.gameObject.name = "[Preview Character]";
            
            second.gameObject.transform.position = previewCharacterPosition;
            
            dragRotate.Target = character.transform;
        }
    }
}

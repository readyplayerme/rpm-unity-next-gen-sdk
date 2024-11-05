using UnityEngine;
using System.Linq;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;

namespace ReadyPlayerMe.Demo
{
    public class CategoryController : MonoBehaviour
    {
        [SerializeField] private CategoryButton categoryButtonPrefab;
        [SerializeField] private Transform categoryButtonContainer;
        [SerializeField] private List<Sprite> categoryIcons = new List<Sprite>();

        private AssetApi assetApi;
        private AudioSource audioSource;
        private List<CategoryButton> categoryButtons = new List<CategoryButton>();
        private Dictionary<string, Sprite> categoryIconDict = new Dictionary<string, Sprite>();
        
        private string[] mainCategories = new string[]
        {
            "baseModel",
        };

        // Map category names to icons for easy lookup
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            categoryIconDict = categoryIcons.ToDictionary(x => x.name.ToLower());
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
        }

        /// <summary>
        ///     Load asset categories from the API and create buttons for each category.
        /// </summary>
        public async void LoadCategories()
        {
            assetApi = new AssetApi();

            AssetTypeListResponse response = await assetApi.ListAssetTypesAsync(new AssetTypeListRequest());
            var categories = response.Data;
            
            // reorder buttons to match main categories order
            categories = mainCategories.Concat(categories.Except(mainCategories)).ToArray();

            foreach (string category in categories)
            {
                CategoryButton button = Instantiate(categoryButtonPrefab, categoryButtonContainer);
                Sprite icon = categoryIconDict["custom"];

                foreach (var item in categoryIconDict)
                {
                    if (category.ToLower().Contains(item.Key.ToLower()))
                    {
                        icon = item.Value;
                        break;
                    }
                }
                
                button.Initialize(category, icon, audioSource);
                categoryButtons.Add(button);
            }

            EventAggregator.Instance.RaiseCategoriesLoaded();
        }
        
        private void OnCategorySelected(string category)
        {
            foreach (CategoryButton button in categoryButtons)
            {
                bool isSelected = button.Category == category;
                button.SetSelected(isSelected);
            }
        }
        
        public void SelectFirstCategory()
        {
            if (categoryButtons.Count > 0)
            {
                categoryButtons[0].OnPointerClick(null);
            }
        }
    }
}
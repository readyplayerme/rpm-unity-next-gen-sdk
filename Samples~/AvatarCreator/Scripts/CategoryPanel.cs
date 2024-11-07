using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Api.V1;
using UnityEngine;
using UnityEngine.Events;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    [Serializable]
    struct CategoryIconAsset
    {
        public string Name;
        public Sprite Icon;
    }
    
    public class CategoryPanel : MonoBehaviour
    {
        public UnityEvent<string> OnCategorySelected;
        public UnityEvent<string[]> OnCategoriesFetched;
        
        [SerializeField] private CategoryButton categoryButtonPrefab;
        [SerializeField] private Transform categoryButtonContainer;
        [SerializeField] private CategoryIconAsset[] categoryIconAssets;
        
        private AssetApi assetApi;
        private CategoryButton selectedCategoryButton;
        private List<CategoryButton> categoryButtons = new List<CategoryButton>();

        private void OnDestroy()
        {
            foreach (var categoryButton in categoryButtons)
            {
                Destroy( categoryButton.gameObject );
            }
            categoryButtons.Clear();
        }

        public async void LoadCategories()
        {
            if (assetApi == null)
            {
                assetApi = new AssetApi();
            }

            AssetTypeListResponse response = await assetApi.ListAssetTypesAsync(new AssetTypeListRequest());
            var categories = response.Data;
            OnCategoriesFetched?.Invoke(categories);
            foreach (string category in categories)
            {
                Debug.Log($"Category: {category}");
                var button = Instantiate(categoryButtonPrefab, categoryButtonContainer);
                
                CategoryIconAsset? categoryIconAsset = categoryIconAssets.FirstOrDefault(x => category.Contains(x.Name));
                Sprite categoryIcon = categoryIconAsset.Value.Icon;
                
                if (categoryIcon == null)
                {
                    Debug.Log($"No icon found for category: {category}. Initializing with null icon.");
                }
                else
                {
                    Debug.Log($"Category: {category}, Icon: {categoryIcon.name}");
                }
                button.Initialize(category, categoryIcon);
                button.OnCategorySelected += HandlecategorySelected;
                categoryButtons.Add(button);
            }
        }

        private void HandlecategorySelected(CategoryButton category)
        {
            if(selectedCategoryButton != null)
            {
                selectedCategoryButton.SetSelected(false);
            }
            selectedCategoryButton = category;
            OnCategorySelected?.Invoke(category.Category);
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
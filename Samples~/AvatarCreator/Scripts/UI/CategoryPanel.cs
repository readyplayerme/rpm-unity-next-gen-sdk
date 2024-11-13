using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Api.V1;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    [Serializable]
    struct CategoryIconAsset
    {
        public string CategoryName;
        public string DisplayName;
    }
    
    public class CategoryPanel : MonoBehaviour
    {
        public UnityEvent<string> OnCategorySelected;
        public UnityEvent<string[]> OnCategoriesFetched;
        
        [FormerlySerializedAs("categoryButtonPrefab"),SerializeField] private CategoryTextButton categoryTextButtonPrefab;
        [SerializeField] private Transform categoryButtonContainer;
        [SerializeField] private CategoryIconAsset[] categoryIconAssets;
        
        private AssetApi assetApi;
        private CategoryTextButton selectedCategoryTextButton;
        private List<CategoryTextButton> categoryButtons = new List<CategoryTextButton>();

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
            foreach (var category in categories)
            {
                var button = Instantiate(categoryTextButtonPrefab, categoryButtonContainer);
                CategoryIconAsset? categoryIconAsset = categoryIconAssets.FirstOrDefault(x => category.Contains(x.CategoryName));
                button.Initialize(category, categoryIconAsset.Value.DisplayName ?? category);
                button.OnCategorySelected += HandlecategorySelected;
                categoryButtons.Add(button);
            }
        }

        private void HandlecategorySelected(CategoryTextButton categoryText)
        {
            if(selectedCategoryTextButton != null)
            {
                selectedCategoryTextButton.SetSelected(false);
            }
            selectedCategoryTextButton = categoryText;
            OnCategorySelected?.Invoke(categoryText.Category);
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
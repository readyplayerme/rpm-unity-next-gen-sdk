using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Api.V1;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        [SerializeField] private CategoryTextButton categoryTextButtonPrefab;
        [SerializeField] private Transform categoryButtonContainer;
        [SerializeField] private CategoryIconAsset[] categoryIconAssets;
        [SerializeField] private ScrollRect scrollRect;
        
        public string[] GetCategoriesInOrder()
        {
            return categoryIconAssets.Select(x => x.CategoryName).ToArray();
        }
        private AssetApi assetApi;
        private CategoryTextButton selectedCategoryTextButton;
        private List<CategoryTextButton> categoryButtons = new List<CategoryTextButton>();

        // inspector label
        [Header("Events")]
        public UnityEvent<string> OnCategorySelected;
        public UnityEvent<string[]> OnCategoriesFetched;
        
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
                button.name = category;
                CategoryIconAsset? categoryIconAsset = categoryIconAssets.FirstOrDefault(x => category.Contains(x.CategoryName));
                button.Initialize(category, categoryIconAsset.Value.DisplayName ?? category);
                button.OnCategorySelected += HandleCategorySelected;
                categoryButtons.Add(button);
            }
            ReorderCategories();
            SelectFirstChildCategory();
        }

        private void HandleCategorySelected(CategoryTextButton categoryText)
        {
            if(selectedCategoryTextButton != null)
            {
                selectedCategoryTextButton.SetSelected(false);
            }
            selectedCategoryTextButton = categoryText;
            OnCategorySelected?.Invoke(categoryText.Category);
            SnapToSelected();
        }
        
        public void SelectFirstChildCategory()
        {
            if (categoryButtonContainer.childCount > 0)
            {
                categoryButtonContainer.GetChild(0).GetComponent<CategoryTextButton>().OnPointerClick(null);
            }
            if(scrollRect != null)
            {
                scrollRect.normalizedPosition = new Vector2(0, 1);
            }
        }
        
        public void ReorderCategories()
        {
            // Loop through the array and find the corresponding child in the container
            for (int i = 0; i < categoryIconAssets.Length; i++)
            {
                // Find the child with the matching name
                Transform childToReorder = null;
                foreach (var categoryButton in categoryButtons)
                {
                    if (categoryButton.name.Contains(categoryIconAssets[i].CategoryName))
                    {
                        childToReorder = categoryButton.transform;
                        break;
                    }
                }

                // If we found a matching child, set its sibling index to the desired position
                if (childToReorder != null)
                {
                    childToReorder.SetSiblingIndex(i);
                }
            }
        }

        public void SnapToSelected()
        {
            var scrollRectPosition = scrollRect.content.position;
            var selectedCategoryPosition = selectedCategoryTextButton.transform.position;
            if (!scrollRect.horizontal)
            {
                scrollRectPosition.x = 0;
                selectedCategoryPosition.x = 0;
                
            }
            if (!scrollRect.vertical)
            {
                scrollRectPosition.y = 0;
                selectedCategoryPosition.y = 0;
            }
            // var anchoredPosition = scrollRect.content.anchoredPosition;
            // Debug.Log($" scrollRectPosition: {scrollRectPosition} selectedCategoryPosition: {selectedCategoryPosition} anchoredPosition: {anchoredPosition}");
            scrollRect.content.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(scrollRectPosition) -
                (Vector2)scrollRect.transform.InverseTransformPoint(selectedCategoryPosition);
        }
    }
}
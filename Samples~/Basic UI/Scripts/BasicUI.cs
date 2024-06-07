using UnityEngine;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Samples.BasicUI
{
    public class BasicUI : MonoBehaviour
    {
        [SerializeField] private CategoryController categoryController;
        [SerializeField] private AssetPageController assetPageController;
        [SerializeField] private AssetCard assetCard;
        
        private string selectedCategory;

        private void Start()
        {
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
            EventAggregator.Instance.OnAssetSelected += OnAssetSelected;
            EventAggregator.Instance.OnPageChanged += OnPageChanged;
            categoryController.LoadCategories();
        }

        private void OnCategorySelected(string category)
        {
            selectedCategory = category;
            assetPageController.LoadAssets(category);
        }
        
        private void OnAssetSelected(Asset asset)
        {
            assetCard.Initialize(asset);
        }
        
        private void OnPageChanged(int page)
        {
            assetPageController.LoadAssets(selectedCategory, page);
        }
    }
}

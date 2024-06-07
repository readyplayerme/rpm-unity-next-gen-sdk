using UnityEngine;
using System.Linq;
using ReadyPlayerMe.Api.V1;
using System.Collections.Generic;

namespace ReadyPlayerMe.Samples.BasicUI
{
    public class CategoryController : MonoBehaviour
    {
        [SerializeField] private CategoryButton categoryButtonPrefab;
        [SerializeField] private Transform categoryButtonContainer;
        [SerializeField] private List<Sprite> categoryIcons = new List<Sprite>();

        private AssetApi assetApi;
        private Dictionary<string, Sprite> categoryIconDict = new Dictionary<string, Sprite>();

        // Map category names to icons for easy lookup
        private void Start()
        {
            categoryIconDict = categoryIcons.ToDictionary(x => x.name.ToLower());
        }

        /// <summary>
        ///     Load asset categories from the API and create buttons for each category.
        /// </summary>
        public async void LoadCategories()
        {
            assetApi = new AssetApi();

            AssetTypeListResponse response = await assetApi.ListAssetTypesAsync(new AssetTypeListRequest());
            var categories = response.Data;

            foreach (string category in categories)
            {
                CategoryButton button = Instantiate(categoryButtonPrefab, categoryButtonContainer);
                categoryIconDict.TryGetValue(category.ToLower(), out Sprite value);
                Sprite icon = value ?? categoryIconDict["custom"];
                button.Initialize(category, icon);
            }

            EventAggregator.Instance.RaiseCategoriesLoaded();
        }
    }
}
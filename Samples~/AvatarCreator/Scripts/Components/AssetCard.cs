using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;
using System;

namespace ReadyPlayerMe.Demo
{
    public class AssetCard : MonoBehaviour
    {
        [SerializeField] private Text assetCategory;
        [SerializeField] private Text assetName;
        [SerializeField] private Image assetImage;
        [SerializeField] private Sprite emptySprite;

        private void Start()
        {
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
        }

        /// <summary>
        ///     Initialize the asset card with the given asset.
        /// </summary>
        /// <param name="asset">Asset related to this button.</param>
        public void Initialize(Asset asset)
        {
            assetCategory.text = asset.Type;
            assetName.text = asset.Name;
            LoadImageAsync(asset.IconUrl);
        }

        // Loads the image from the given URL asynchronously.
        private async void LoadImageAsync(string url)
        {
            FileApi fileApi = new FileApi();
            Texture2D texture = await fileApi.DownloadImageAsync(url);
            assetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        // Slightly moves the asset card around.
        private void Update()
        {
            float x = Mathf.Sin(Time.time) * 10;
            float y = Mathf.Cos(Time.time) * 10;
            assetImage.rectTransform.anchoredPosition = new Vector2(x, y);
        }
        
        // Resets the asset card when a category is selected.
        private void OnCategorySelected(string category)
        {
            assetCategory.text = null;
            assetName.text = null;
            assetImage.sprite = emptySprite;
        }
    }
}

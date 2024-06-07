using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Samples
{
    public class AssetCard : MonoBehaviour
    {
        [SerializeField] private Text assetCategory;
        [SerializeField] private Text assetName;
        [SerializeField] private Image assetImage;
        [SerializeField] private Text assetDescription;

        /// <summary>
        ///     Initialize the asset card with the given asset.
        /// </summary>
        /// <param name="asset">Asset related to this button.</param>
        public void Initialize(Asset asset)
        {
            assetCategory.text = asset.Type;
            assetName.text = asset.Id;
            assetDescription.text = asset.Id;
            LoadImageAsync(asset.IconUrl);
        }

        // Loads the image from the given URL asynchronously.
        private async void LoadImageAsync(string url)
        {
            FileApi fileApi = new FileApi();
            Texture2D texture = await fileApi.DownloadImageAsync(url);
            assetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}

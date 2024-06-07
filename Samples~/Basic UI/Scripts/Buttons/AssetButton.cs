using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Samples.BasicUI
{
    public class AssetButton : MonoBehaviour
    {
        [SerializeField] private Image assetIcon;
        [SerializeField] private Button button;

        /// <summary>
        ///     Initializes the asset button with the given asset.
        /// </summary>
        /// <param name="asset">Asset data.</param>
        public void Initialize(Asset asset)
        {
            button.onClick.AddListener(() => EventAggregator.Instance.RaiseAssetSelected(asset));
            LoadImageAsync(asset.IconUrl);
        }

        // Loads the image from the given URL asynchronously.
        private async void LoadImageAsync(string url)
        {
            FileApi fileApi = new FileApi();
            Texture2D texture = await fileApi.DownloadImageAsync(url);
            assetIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}
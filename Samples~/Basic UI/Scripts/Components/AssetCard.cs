using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
            StartCoroutine(LoadImageAsync(asset.IconUrl));
        }

        // TODO: Move this method to SDK
        private IEnumerator LoadImageAsync(string url)
        {
            WWW www = new WWW(url);

            yield return www;
            assetImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
                Vector2.zero);
        }
    }
}

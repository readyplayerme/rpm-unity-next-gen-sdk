using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
            StartCoroutine(LoadImageAsync(asset.IconUrl));
        }

        // TODO: Move this to SDK Itself
        private IEnumerator LoadImageAsync(string url)
        {
            WWW www = new WWW(url);

            yield return www;
            assetIcon.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
                Vector2.zero);
        }
    }
}
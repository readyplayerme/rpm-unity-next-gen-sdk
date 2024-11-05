using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ReadyPlayerMe.Api.V1;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe.Demo
{
    public class AssetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite selectedSprite;

        [Header("UI Elements")] [SerializeField]
        private Image buttonImage;

        [SerializeField] private Image lineImage;
        [SerializeField] private Image assetIcon;
        [SerializeField] private GameObject loadingIcon;

        [Header("Button Colors")] [SerializeField]
        private Color normalColor = new Color(0.3f, 0.37f, 0.98f, 1);

        [SerializeField] private Color hoverColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color selectedColor = new Color(0.06f, 0.52f, 0.11f, 1);

        [Header("Line Colors")] [SerializeField]
        private Color normalLineColor = new Color(0.25f, 0.3f, 0.8f, 1);

        [SerializeField] private Color hoverLineColor = new Color(0.8f, 0.8f, 0.9f, 1);
        [SerializeField] private Color selectedLineColor = new Color(0.06f, 0.52f, 0.11f, 1);

        [Header("Sound Effects")] [SerializeField]
        private AudioClip hoverSfx;

        [SerializeField] private AudioClip clickSfx;

        private bool isSelected;
        private AudioSource audioSource;
        public Asset Asset { get; private set; }

        public void Initialize(Asset asset, AudioSource audioSource)
        {
            Asset = asset;
            this.audioSource = audioSource;

            buttonImage.sprite = normalSprite;
            buttonImage.color = normalColor;
            lineImage.color = normalLineColor;
            LoadImageAsync(asset.IconUrl);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(OnPointerEnterAsync());
            audioSource.PlayOneShot(hoverSfx);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(OnPointerExitAsync());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // skip if already selected
            if (isSelected) return;
            
            EventAggregator.Instance.RaiseAssetSelected(Asset);
            audioSource.PlayOneShot(clickSfx);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            buttonImage.sprite = selected ? selectedSprite : normalSprite;
            buttonImage.color = selected ? selectedColor : normalColor;
            lineImage.color = selected ? selectedLineColor : normalLineColor;
        }

        private IEnumerator OnPointerEnterAsync()
        {
            Color lineColor = isSelected ? selectedLineColor : normalLineColor;
            Color buttonColor = isSelected ? selectedColor : normalColor;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * 5;
                buttonImage.color = Color.Lerp(buttonColor, hoverColor, progress);
                lineImage.color = Color.Lerp(lineColor, hoverLineColor, progress);
                assetIcon.transform.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, progress);
                lineImage.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 1, 1), progress);

                yield return null;
            }
        }

        private IEnumerator OnPointerExitAsync()
        {
            Color lineColor = isSelected ? selectedLineColor : normalLineColor;
            Color buttonColor = isSelected ? selectedColor : normalColor;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * 5;
                buttonImage.color = Color.Lerp(hoverColor, buttonColor, progress);
                lineImage.color = Color.Lerp(hoverLineColor, lineColor, progress);
                assetIcon.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.9f, progress);
                lineImage.transform.localScale = Vector3.Lerp(new Vector3(1.5f, 1, 1), Vector3.one, progress);

                yield return null;
            }
        }

        private async void LoadImageAsync(string url)
        {
            FileApi fileApi = new FileApi();
            Texture2D texture = await fileApi.DownloadImageAsync(url);
            assetIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            loadingIcon.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe.Demo
{
    [RequireComponent(typeof(Image))]
    public class CategoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image iconImage;

        [Header("Button Colors")] [SerializeField]
        private Color normalButtonColor = new Color(1, 1, 1, 0.2f);

        [SerializeField] private Color hoverButtonColor = new Color(1, 1, 1, 0.6f);
        [SerializeField] private Color selectedButtonColor = new Color(1, 1, 1, 1);

        [Header("Icon Colors")] [SerializeField]
        private Color normalIconColor = Color.white;

        [SerializeField] private Color hoverIconColor = new Color(0.2f, 0.2f, 0.4f, 1f);
        [SerializeField] private Color selectedIconColor = new Color(0.2f, 0.2f, 0.4f, 1);

        [Header("Sound Effects")] [SerializeField]
        private AudioClip hoverSfx;

        [SerializeField] private AudioClip clickSfx;

        private bool isSelected;
        private AudioSource audioSource;
        public string Category { get; private set; } = string.Empty;

        public void Initialize(string category, Sprite icon, AudioSource audioSource)
        {
            Category = category;
            this.audioSource = audioSource;

            iconImage.sprite = icon;
            iconImage.color = normalIconColor;
            buttonImage.color = normalButtonColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(LerpAsync(buttonImage, hoverButtonColor, 0.2f));
            StartCoroutine(LerpAsync(iconImage, hoverIconColor, 0.2f));
            audioSource.PlayOneShot(hoverSfx);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            buttonImage.color = isSelected ? selectedButtonColor : normalButtonColor;
            iconImage.color = isSelected ? selectedIconColor : normalIconColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(LerpAsync(buttonImage, selectedButtonColor, 0.2f));
            EventAggregator.Instance.RaiseCategorySelected(Category);
            audioSource.PlayOneShot(clickSfx);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            StopAllCoroutines();
            StartCoroutine(LerpAsync(iconImage, selected ? selectedIconColor : normalIconColor, 0.2f));
            StartCoroutine(LerpAsync(buttonImage, selected ? selectedButtonColor : normalButtonColor, 0.2f));
        }

        private IEnumerator LerpAsync(Image source, Color target, float duration)
        {
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime / duration;
                source.color = Color.Lerp(source.color, target, progress);

                yield return null;
            }
        }
    }
}

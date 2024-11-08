using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class ButtonInteractions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
   
        [SerializeField] private Image assetImage;
        
        [SerializeField] private Sprite normalButtonSprite;
        [SerializeField] private Sprite selectedButtonSprite;
        

        [Header("Button Colors")] [SerializeField]
        private Color normalColor = new Color(0.3f, 0.37f, 0.98f, 1);
        [SerializeField] private Color hoverColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color selectedColor = new Color(0.06f, 0.52f, 0.11f, 1);
        
        [Header("Sound Effects")] 
        [SerializeField] private AudioClip hoverSfx;
        [SerializeField] private AudioClip clickSfx;
        
        private Image buttonImage;
        
        private AudioSource audioSource;
        private ISelectable selectable;
        
        private void Start()
        {
            buttonImage = GetComponent<Image>();
            selectable = GetComponent<ISelectable>();
            selectable.OnSelectionChanged += SetSelected;
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null) return;
            Debug.LogWarning($"AudioSource not found. Adding AudioSource to the GameObject.{gameObject.name}");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void SetSelected(bool isSelected)
        {
            if (selectedButtonSprite != null || normalButtonSprite != null)
            {
                buttonImage.sprite = isSelected ? selectedButtonSprite : normalButtonSprite;
            }
            buttonImage.color = isSelected ? selectedColor : normalColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hoverSfx != null) audioSource.PlayOneShot(hoverSfx);
            if(selectable.IsSelected) return;
            StopAllCoroutines();
            StartCoroutine(OnPointerEnterAsync());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(selectable.IsSelected) return;
            StopAllCoroutines();
            StartCoroutine(OnPointerExitAsync());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(clickSfx != null) audioSource.PlayOneShot(clickSfx);
        }
        
        private IEnumerator OnPointerEnterAsync()
        {
            //Color lineColor = selectable.IsSelected ? selectedLineColor : normalLineColor;
            var buttonColor = selectable.IsSelected ? selectedColor : normalColor;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * 5;
                buttonImage.color = Color.Lerp(buttonColor, hoverColor, progress);
                //lineImage.color = Color.Lerp(lineColor, hoverLineColor, progress);
                assetImage.transform.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, progress);
                //lineImage.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 1, 1), progress);

                yield return null;
            }
        }

        private IEnumerator OnPointerExitAsync()
        {
            //Color lineColor = isSelected ? selectedLineColor : normalLineColor;
            var buttonColor = selectable.IsSelected ? selectedColor : normalColor;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * 5;
                buttonImage.color = Color.Lerp(hoverColor, buttonColor, progress);
                //lineImage.color = Color.Lerp(hoverLineColor, lineColor, progress);
                assetImage.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.9f, progress);
                //lineImage.transform.localScale = Vector3.Lerp(new Vector3(1.5f, 1, 1), Vector3.one, progress);

                yield return null;
            }
        }
    }
}
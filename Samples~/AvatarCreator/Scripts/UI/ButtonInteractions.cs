using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class ButtonInteractions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image assetImage;
        
        [Header("Sound Effects")] 
        [SerializeField] private AudioClip hoverSfx;
        [SerializeField] private AudioClip clickSfx;
        
        private AudioSource audioSource;
        private ISelectable selectable;
        private Animator animator;
        private static readonly int ButtonStateKey = Animator.StringToHash("ButtonState");

        private void Start()
        {
            animator = GetComponent<Animator>();
            animator.keepAnimatorStateOnDisable = true;
            selectable = GetComponent<ISelectable>();
            selectable.OnSelectionChanged += SetSelected;
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null) return;
            Debug.LogWarning($"AudioSource not found. Adding AudioSource to the GameObject.{gameObject.name}");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void SetSelected(bool isSelected)
        {
            animator.SetInteger( ButtonStateKey, isSelected ? 2 : 0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hoverSfx != null) audioSource.PlayOneShot(hoverSfx);
            animator.SetInteger( ButtonStateKey, selectable.IsSelected ? 2 : 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            animator.SetInteger( ButtonStateKey, selectable.IsSelected ? 2 : 0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(clickSfx != null) audioSource.PlayOneShot(clickSfx);
            animator.SetInteger( ButtonStateKey, selectable.IsSelected ? 2 : 1);
        }
    }
}
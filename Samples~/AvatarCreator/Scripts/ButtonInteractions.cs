using System;
using System.Collections;
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
        private int animState = 0;

        private void Start()
        {
            
            selectable = GetComponent<ISelectable>();
            selectable.OnSelectionChanged += SetSelected;
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null) return;
            Debug.LogWarning($"AudioSource not found. Adding AudioSource to the GameObject.{gameObject.name}");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                animator.keepAnimatorStateOnDisable = true;
            }
            animator.SetInteger( ButtonStateKey,  animState);
        }

        private void OnDisable()
        {
            
        }

        private void SetSelected(bool isSelected)
        {
            animState = isSelected ? 2 : 0;
            animator.SetInteger( ButtonStateKey, animState);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hoverSfx != null) audioSource.PlayOneShot(hoverSfx);
            if(selectable.IsSelected) return;
            animState = selectable.IsSelected ? 0 : 1;
            animator.SetInteger( ButtonStateKey,  animState);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            animState = selectable.IsSelected ? 2 : 0;
            animator.SetInteger( ButtonStateKey, animState);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            animState = selectable.IsSelected ? 2 : 1;
            if(clickSfx != null) audioSource.PlayOneShot(clickSfx);
            animator.SetInteger( ButtonStateKey, animState);
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe
{
    [RequireComponent(typeof(AudioSource))]
    public class ButtonSounds : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private AudioClip hoverSfx;
        [SerializeField] private AudioClip clickSfx;
        
        private AudioSource audioSource;
        
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null) return;
            Debug.LogWarning($"AudioSource not found. Adding AudioSource to the GameObject.{gameObject.name}");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(clickSfx != null) audioSource.PlayOneShot(clickSfx);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hoverSfx != null) audioSource.PlayOneShot(hoverSfx);
        }
    }
}
using ReadyPlayerMe.Api.V1;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe.Demo
{
    public class EquipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image buttonBackground;
        [SerializeField] private GameObject loadingSpinner;
        [SerializeField] private Text buttonLabel;
        
        [Header("Sprites")]
        [SerializeField] private Sprite equip;
        [SerializeField] private Sprite unequip;
        [SerializeField] private Sprite loading;

        // Colors
        private Color normalColor = new Color(1, 1, 1, 0.6f);
        private Color hoverColor = new Color(1, 1, 1, 1f);

        [Header("Sound Effects")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hoverSfx;
        [SerializeField] private AudioClip equipSfx;
        [SerializeField] private AudioClip unequipSfx;
        
        private bool isEquipped;
        private bool isLoading;
        private Asset asset;

        private void Start()
        {
            EventAggregator.Instance.OnCategorySelected += OnCategorySelected;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            buttonBackground.color = hoverColor;
            audioSource.PlayOneShot(hoverSfx);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            buttonBackground.color = normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(isLoading) return;
            
            if (isEquipped)
            {
                ToggleEquipState(false);
                audioSource.PlayOneShot(unequipSfx);
                EventAggregator.Instance.RaiseAssetEquipped(asset);
            }
            else
            {
                isEquipped = true;
                audioSource.PlayOneShot(equipSfx);
                EventAggregator.Instance.RaiseAssetUnequipped(asset);
            }
        }
        
        public void Loading(Asset asset)
        {
            isLoading = true;
            buttonBackground.sprite = loading;
            buttonLabel.text = "";
            loadingSpinner.SetActive(true);
            this.asset = asset;
        }

        public void ToggleEquipState(bool equiped)
        {
            isLoading = false;
            isEquipped = equiped;
            buttonBackground.sprite = equiped ? equip : unequip;
            buttonLabel.text = equiped ? "EQUIP" : "UNEQUIP";
            loadingSpinner.SetActive(false);
        }
        
        private void OnCategorySelected(string category)
        {
            ToggleEquipState(false);
            gameObject.SetActive(false);
        }
    }
}

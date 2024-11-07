using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class CategoryButton : MonoBehaviour, IPointerClickHandler
    {
        public Action<CategoryButton> OnCategorySelected;
        
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image iconImage;

        private bool isSelected;
        public string Category { get; private set; } = string.Empty;

        public void Initialize(string category, Sprite icon)
        {
            Category = category;
            if(icon == null)
            {
                Debug.LogError($"Icon not found for category: {category}");
                return;
            }
            iconImage.sprite = icon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelected(!isSelected);
            OnCategorySelected?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
        }
    }
}
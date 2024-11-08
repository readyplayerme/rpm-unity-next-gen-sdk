using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class CategoryButton : MonoBehaviour, IPointerClickHandler, ISelectable
    {
        public Action<CategoryButton> OnCategorySelected;
        
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image iconImage;
        public string Category { get; private set; } = string.Empty;
        
        public bool IsSelected { get; protected set; }
        public Action<bool> OnSelectionChanged { get; set; }

        public void Initialize(string category, Sprite icon)
        {
            Category = category;
            if(icon == null)
            {
                return;
            }
            iconImage.sprite = icon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelected(!IsSelected);
            OnCategorySelected?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            OnSelectionChanged?.Invoke(IsSelected);
        }
    }
}
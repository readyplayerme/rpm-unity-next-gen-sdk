using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class CategoryTextButton : MonoBehaviour, IPointerClickHandler, ISelectable
    {
        public Action<CategoryTextButton> OnCategorySelected;
        
        [SerializeField] private Image buttonImage;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private bool disableDeselectOnClick = true;

        public string Category { get; private set; } = string.Empty;
        
        public bool IsSelected { get; protected set; }
        public Action<bool> OnSelectionChanged { get; set; }

        public void Initialize(string category, string newText)
        {
            Category = category;
            if(buttonText == null)
            {
                return;
            }
            buttonText.text = newText;

            
            var textWidth = CalculateTextWidth(buttonText);
            var padding = buttonText.margin.x + buttonText.margin.z; 
            
            var buttonRectTransform = GetComponent<RectTransform>();
            buttonRectTransform.sizeDelta = new Vector2(textWidth + padding, buttonRectTransform.sizeDelta.y);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (disableDeselectOnClick && IsSelected) return;
            SetSelected(!IsSelected);
            OnCategorySelected?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            OnSelectionChanged?.Invoke(IsSelected);
        }
        
        private float CalculateTextWidth(TMP_Text textComponent)
        {
            // Get the preferred width for the current text
            Vector2 textSize = textComponent.GetPreferredValues(textComponent.text);
            return textSize.x;
        }
    }
}
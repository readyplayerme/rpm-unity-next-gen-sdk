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
        [FormerlySerializedAs("text"),SerializeField] private TMP_Text buttonText;
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

            
            var margins = buttonText.margin;
            var textWidth = CalculateTextWidth(buttonText);

            // Adjust the button width based on the text width
            RectTransform buttonRectTransform = GetComponent<RectTransform>();
            buttonRectTransform.sizeDelta = new Vector2(textWidth, buttonRectTransform.sizeDelta.y);
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
        
        private float CalculateTextWidth(TMP_Text textComponent)
        {
            // Get the preferred width for the current text
            Vector2 textSize = textComponent.GetPreferredValues(textComponent.text);
            return textSize.x;
        }
    }
}
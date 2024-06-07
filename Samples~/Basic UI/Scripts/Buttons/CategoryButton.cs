using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.BasicUI
{
    /// <summary>
    ///     Represents a category button in the category selection menu.
    /// </summary>
    public class CategoryButton : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        /// <summary>
        ///     Initializes the category button with the given category and icon.
        /// </summary>
        /// <param name="category">Asset category.</param>
        /// <param name="icon">Category icon.</param>
        public void Initialize(string category, Sprite icon)
        {
            iconImage.sprite = icon;
            button.onClick.AddListener(() => EventAggregator.Instance.RaiseCategorySelected(category));
        }
    }
}
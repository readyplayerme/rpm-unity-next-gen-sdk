using UnityEngine;
using UnityEngine.UI;
using ReadyPlayerMe.Api.V1;

namespace ReadyPlayerMe.Demo
{
    public class Paginator : MonoBehaviour
    {
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Text pageText;
        [SerializeField] private AudioClip pageChangeSfx;

        private Pagination pagination;
        private AudioSource audioSource;

        // Subscribe to the button events
        private void Start()
        {
            prevButton.onClick.AddListener(() => ChangePage(-1));
            nextButton.onClick.AddListener(() => ChangePage(1));
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        ///     Initialize the paginator with the pagination data.
        /// </summary>
        /// <param name="pagination">Pagination data of selected category.</param>
        public void Initialize(Pagination pagination)
        {
            this.pagination = pagination;
            SetButtons();
        }

        // Update the buttons based on the current page
        private void SetButtons()
        {
            pageText.text = $"{pagination.Page}/{pagination.TotalPages}";
            nextButton.interactable = pagination.HasNextPage;
            prevButton.interactable = pagination.HasPrevPage;
        }

        // Change the page and update the buttons
        private void ChangePage(int index)
        {
            audioSource.PlayOneShot(pageChangeSfx);
            EventAggregator.Instance.RaisePageChanged(pagination.Page + index);
            SetButtons();
        }
    }
}

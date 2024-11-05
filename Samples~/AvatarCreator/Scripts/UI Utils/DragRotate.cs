using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe.Demo
{
    public class DragRotate : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        public Transform Target { get; set; }

        private bool isDragging;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isDragging && Target != null)
            {
                float x = eventData.delta.x / Screen.width * rectTransform.rect.width;
                float dragSpeed = Mathf.Clamp(x, -1, 1);
                Target.Rotate(Vector3.up, -dragSpeed * 5f);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }
    }
}

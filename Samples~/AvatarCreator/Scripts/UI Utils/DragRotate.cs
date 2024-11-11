using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyPlayerMe.Demo
{
    public class DragRotate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Transform Target { get; set; }

        private bool isDragging;
        private Vector3 lastMousePosition;

        private void Update()
        {
            // Only rotate if dragging is active
            if (isDragging && Target != null)
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

                float dragSpeed = Mathf.Clamp(mouseDelta.x / Screen.width, -1, 1);
                Target.Rotate(Vector3.up, -dragSpeed * 500f);

                lastMousePosition = Input.mousePosition;
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log($"Mouse up on {gameObject.name}");
                    isDragging = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }

        public void SetTarget(GameObject target)
        {
            Target = target.transform;
        }
    }
}

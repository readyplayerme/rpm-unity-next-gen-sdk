using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class SimpleThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target; 
        [SerializeField] private float rotationSpeed = 5f; 
        [SerializeField] private bool hideCursorOnStart = true;
        private float currentRotationY;
        
        private void Start()
        {
            if (hideCursorOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void SetTarget(GameObject target)
        {
            this.target = target.transform;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            transform.position = target.position;

            HandleRotation();
        }

        private void HandleRotation()
        {
            var horizontalInput = Input.GetAxis("Mouse X");

            currentRotationY += horizontalInput * rotationSpeed;

            transform.rotation = Quaternion.Euler(0, currentRotationY, 0);
        }

    }
}

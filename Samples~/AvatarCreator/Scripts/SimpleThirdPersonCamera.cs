using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public class SimpleThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target; 
        [SerializeField] private float rotationSpeed = 5f; 
        private float currentRotationY;

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

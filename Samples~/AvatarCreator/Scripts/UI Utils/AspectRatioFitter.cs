using UnityEngine;

namespace ReadyPlayerMe.Demo
{
    public class AspectRatioFitter : MonoBehaviour
    {
        private Camera mainCamera;
        private float cameraAspectRatio;
        private RectTransform canvasRectTransform;
        
        public static float CameraAspectRatio { get; private set; }
        
        private void UpdateScreenSize()
        {
            float canvasHeight = canvasRectTransform.rect.height;
            CameraAspectRatio = mainCamera.aspect;
            float desiredCanvasWidth = canvasHeight * CameraAspectRatio;
            canvasRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredCanvasWidth);
        }

        private void Awake()
        {   
            mainCamera = Camera.main;
            canvasRectTransform = GetComponent<RectTransform>();
            
            UpdateScreenSize();
        }

        private void Update()
        {
            if(!Mathf.Approximately(mainCamera.aspect, cameraAspectRatio))
            {
                UpdateScreenSize();
            }
        }
    }
}

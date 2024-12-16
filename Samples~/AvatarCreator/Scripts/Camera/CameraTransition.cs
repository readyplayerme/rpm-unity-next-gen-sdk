using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField]
    private Transform headView; // Assign the Transform for the head view
    [SerializeField]
    private Transform fullBodyView; // Assign the Transform for the full-body view

    [SerializeField]
    private float transitionDuration = 0.5f; // Duration of the transition

    private bool isTransitioning = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float transitionProgress = 0f;

    public void OnCategoryChanged(string category)
    {
        if (category.Contains("Hair") || category.Contains("Moustache"))
        {
            MoveToTarget(headView);
        }
        else
        {
            MoveToTarget(fullBodyView);
        }
    }

    private void MoveToTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("Target transform is null!");
            return;
        }

        // Set starting and target positions for smooth transition
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetPosition = target.position;
        targetRotation = target.rotation;

        transitionProgress = 0f; // Reset the transition progress
        isTransitioning = true;  // Begin the transition
    }

    private void LateUpdate()
    {
        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            
            transform.position = Vector3.Lerp(startPosition, targetPosition, transitionProgress);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, transitionProgress);
            
            if (transitionProgress >= 1f)
            {
                isTransitioning = false; // Stop the transition
            }
        }
    }
}

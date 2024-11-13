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
        Debug.Log($"Category changed to: {category}");
        
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

        Debug.Log($"Starting transition from {startPosition} to {targetPosition}");
        transitionProgress = 0f; // Reset the transition progress
        isTransitioning = true;  // Begin the transition
    }

    private void LateUpdate()
    {
        if (isTransitioning)
        {
            // Increment the transition progress over time
            transitionProgress += Time.deltaTime / transitionDuration;

            // Lerp position and Slerp rotation
            transform.position = Vector3.Lerp(startPosition, targetPosition, transitionProgress);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, transitionProgress);

            // Log transition details
            Debug.Log($"Transitioning... Progress: {transitionProgress * 100:F1}%, Position: {transform.position}, Rotation: {transform.rotation.eulerAngles}");

            // Check if transition is complete
            if (transitionProgress >= 1f)
            {
                Debug.Log($"Transition complete at {transform.position}");
                isTransitioning = false; // Stop the transition
            }
        }
    }
}

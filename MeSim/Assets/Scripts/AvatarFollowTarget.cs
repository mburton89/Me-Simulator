using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;

public class AvatarFollowTarget : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform cubeTarget;

    [Header("Follow Settings")]
    [SerializeField] private float stopDistance = 0.8f;        // Increased — stop when this close
    [SerializeField] private float runDistanceThreshold = 5f;
    [SerializeField] private float inputSmoothing = 0.1f;       // Smooth input changes to reduce jitter

    private ThirdPersonMovement movement;
    private ThirdPersonController controller;

    private float smoothedInputX;
    private float smoothedInputY;

    private void Awake()
    {
        movement = GetComponent<ThirdPersonMovement>();
        controller = GetComponent<ThirdPersonController>();

        if (controller != null)
            controller.inputEnabled = false; // Disable real keyboard input

        if (movement == null || cubeTarget == null)
        {
            Debug.LogError("AvatarCubeFollower: Missing components or target!");
        }
    }

    private void Update()
    {
        if (movement == null || cubeTarget == null) return;

        Vector3 direction = cubeTarget.position - transform.position;
        float distance = direction.magnitude;

        if (distance < stopDistance)
        {
            // Close enough — stop moving
            SmoothInputTowards(0, 0);
            movement.Move(smoothedInputX, smoothedInputY);
            movement.SetIsRunning(false);
            return;
        }

        // Calculate desired input direction relative to camera
        Vector3 camRight = movement.playerCamera.right;
        Vector3 camForward = Vector3.ProjectOnPlane(movement.playerCamera.forward, Vector3.up).normalized;

        float desiredInputX = Vector3.Dot(direction.normalized, camRight);
        float desiredInputY = Vector3.Dot(direction.normalized, camForward);

        // Smooth the input to prevent sudden changes
        SmoothInputTowards(desiredInputX, desiredInputY);

        // Apply movement
        movement.Move(smoothedInputX, smoothedInputY);
        movement.SetIsRunning(distance > runDistanceThreshold);
    }

    private void SmoothInputTowards(float targetX, float targetY)
    {
        smoothedInputX = Mathf.MoveTowards(smoothedInputX, targetX, inputSmoothing);
        smoothedInputY = Mathf.MoveTowards(smoothedInputY, targetY, inputSmoothing);
    }
}
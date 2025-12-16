using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;

public class AvatarFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform cubeTarget;
    [SerializeField] private float stopDistance = 0.8f;
    [SerializeField] private float runDistanceThreshold = 5f;
    [SerializeField] private float inputSmoothing = 0.1f;

    private ThirdPersonMovement movement;
    private ThirdPersonController controller;
    private CharacterController characterController;

    private float smoothedInputX;
    private float smoothedInputY;

    private void Awake()
    {
        movement = GetComponent<ThirdPersonMovement>();
        controller = GetComponent<ThirdPersonController>();
        characterController = GetComponent<CharacterController>();

        if (controller != null)
            controller.inputEnabled = false;

        if (movement == null || cubeTarget == null || characterController == null)
        {
            Debug.LogError("AvatarFollowTarget: Missing components!");
        }
    }

    private void Update()
    {
        if (movement == null || cubeTarget == null) return;

        Vector3 direction = cubeTarget.position - transform.position;
        float distance = direction.magnitude;

        if (distance < stopDistance)
        {
            SmoothInputTowards(0, 0);
            movement.Move(smoothedInputX, smoothedInputY);
            movement.SetIsRunning(false);
            return;
        }

        // Horizontal camera-relative input
        Vector3 camRight = movement.playerCamera.right;
        Vector3 camForward = Vector3.ProjectOnPlane(movement.playerCamera.forward, Vector3.up).normalized;

        Vector3 horizontalDir = Vector3.ProjectOnPlane(direction, Vector3.up);
        if (horizontalDir.sqrMagnitude > 0.01f)
        {
            float desiredInputX = Vector3.Dot(horizontalDir.normalized, camRight);
            float desiredInputY = Vector3.Dot(horizontalDir.normalized, camForward);

            SmoothInputTowards(desiredInputX, desiredInputY);
        }

        // Apply horizontal movement via QuickStart system
        movement.Move(smoothedInputX, smoothedInputY);
        movement.SetIsRunning(distance > runDistanceThreshold);

        // Direct vertical movement (since gravity is disabled)
        if (Mathf.Abs(direction.y) > 0.05f)
        {
            float verticalSpeed = Mathf.Sign(direction.y) * movement.walkSpeed * 0.8f; // Slightly slower climb
            characterController.Move(Vector3.up * verticalSpeed * Time.deltaTime);
        }

        // Keep upright
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void SmoothInputTowards(float targetX, float targetY)
    {
        smoothedInputX = Mathf.MoveTowards(smoothedInputX, targetX, inputSmoothing);
        smoothedInputY = Mathf.MoveTowards(smoothedInputY, targetY, inputSmoothing);
    }
}
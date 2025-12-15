using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("Drag the PreviewAvatar (or the object with Animator) here")]
    private GameObject controlledObject;

    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject moveIndicatorPrefab;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private ARRaycastManager arRaycastManager;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject currentMoveIndicator;

    // Animation
    [SerializeField] Animator avatarAnimator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;

        if (controlledObject == null)
        {
            Debug.LogWarning("ARTapToMove: No controlledObject assigned!");
            return;
        }

        if (avatarAnimator == null)
        {
            Debug.LogError("ARTapToMove: No Animator found on controlledObject or its children!");
        }
        else
        {
            // Optional: Start in Idle
            avatarAnimator.Play("Idle");
        }
    }

    private void Update()
    {
        HandleTouchInput();

        if (isMoving && controlledObject != null)
        {
            MoveTowardsTarget();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(touch.position, hits, TrackableType.Planes))
        {
            Vector3 hitPoint = hits[0].pose.position;
            SetNewTarget(hitPoint);
        }
    }

    private void SetNewTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;

        // Start Walking animation
        if (avatarAnimator != null)
        {
            avatarAnimator.Play("Walking");
        }

        // Visual indicator
        if (moveIndicatorPrefab != null)
        {
            if (currentMoveIndicator != null)
                Destroy(currentMoveIndicator);

            currentMoveIndicator = Instantiate(moveIndicatorPrefab, targetPosition, Quaternion.identity);
        }
    }

    private void MoveTowardsTarget()
    {
        if (controlledObject == null) return;

        Vector3 direction = targetPosition - controlledObject.transform.position;
        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            // Arrived!
            isMoving = false;

            // Switch to Idle
            if (avatarAnimator != null)
            {
                avatarAnimator.Play("Idle");
            }

            if (currentMoveIndicator != null)
            {
                Destroy(currentMoveIndicator);
                currentMoveIndicator = null;
            }

            return;
        }

        // Continue moving
        Vector3 moveDirection = direction.normalized;
        controlledObject.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotate to face direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            controlledObject.transform.rotation = Quaternion.Slerp(
                controlledObject.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Ensure Walking is playing (in case of interruption)
        if (avatarAnimator != null && avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            avatarAnimator.Play("Walking");
        }
    }
}
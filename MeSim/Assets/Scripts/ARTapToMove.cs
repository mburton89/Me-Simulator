using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("Prefabs & References")]
    [SerializeField] private GameObject objectPrefab;           // Cube prefab (or later: nothing if only using avatar)
    [SerializeField] private Camera arCamera;                    // AR Camera
    [SerializeField] private GameObject moveIndicatorPrefab;    // Optional target marker

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private ARRaycastManager arRaycastManager;
    private GameObject controlledObject;        // The current object being moved (cube or avatar)
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject currentMoveIndicator;

    private Animator currentAnimator; // We'll cache it

    [Header("AR Avatar Control")]
    [SerializeField] private Transform rpmPlayerRoot; // Drag RPM Player here

    // Helper for AvatarLoaderManager to get current position (even if no object yet)
    public Vector3 CurrentObjectPosition
    {
        get { return controlledObject != null ? controlledObject.transform.position : Vector3.zero; }
    }

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;

        targetPosition = Vector3.zero;
    }

    private void Update()
    {
        HandleTouchInput();

        if (rpmPlayerRoot != null && rpmPlayerRoot.childCount > 0)
        {
            controlledObject = rpmPlayerRoot.GetChild(0).gameObject;

            if (isMoving && controlledObject != null)
            {
                MoveTowardsTarget();
            }
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

            // First tap: spawn cube if no object exists yet
            if (controlledObject == null && objectPrefab != null)
            {
                SpawnObject(hitPoint);
            }

            // Always set new target on valid tap
            SetNewTarget(hitPoint);
        }
    }

    private void SpawnObject(Vector3 position)
    {
        controlledObject = Instantiate(objectPrefab, position, Quaternion.identity);
        Debug.Log("Cube spawned at first tap!");
    }

    private void SetNewTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;

        // Visual feedback
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
            isMoving = false;
            if (currentMoveIndicator != null)
            {
                Destroy(currentMoveIndicator);
                currentMoveIndicator = null;
            }

            // Stopped → Idle
            SetAnimationSpeed(0f);
            return;
        }

        // Moving → Walk
        SetAnimationSpeed(1f);

        Vector3 moveDirection = direction.normalized;
        controlledObject.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            controlledObject.transform.rotation = Quaternion.Slerp(
                controlledObject.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void SetAnimationSpeed(float speed)
    {
        // Find and cache animator if needed
        if (currentAnimator == null || currentAnimator.gameObject != controlledObject)
        {
            currentAnimator = controlledObject.GetComponentInChildren<Animator>();
        }

        if (currentAnimator != null)
        {
            currentAnimator.SetFloat("Speed", speed);
        }
    }

    // Called by AvatarLoaderManager when RPM avatar is ready
    public void SwitchToNewObject(GameObject newObject)
    {
        if (controlledObject != null)
        {
            Destroy(controlledObject); // Remove cube
        }

        controlledObject = newObject;

        // If we were mid-movement, keep going toward the same target
        if (isMoving)
        {
            SetNewTarget(targetPosition); // Re-show indicator if needed
        }

        Debug.Log("Switched to new object (RPM Avatar)!");

        controlledObject = newObject;
        currentAnimator = null; // Will be refreshed on next SetAnimationSpeed call

        if (isMoving)
        {
            SetNewTarget(targetPosition);
            SetAnimationSpeed(1f); // Resume walking if mid-movement
        }
        else
        {
            SetAnimationSpeed(0f);
        }
    }
}
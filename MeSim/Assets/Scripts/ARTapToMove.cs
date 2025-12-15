using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("Drag the object you want to move here (must already be in the scene)")]
    private GameObject controlledObject;

    [SerializeField] private Camera arCamera;                    // AR Camera (usually Main Camera)
    [SerializeField] private GameObject moveIndicatorPrefab;    // Optional visual marker at target point

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private ARRaycastManager arRaycastManager;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject currentMoveIndicator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;

        // Optional safety check
        if (controlledObject == null)
        {
            Debug.LogWarning("ARTapToMove: No controlledObject assigned! Drag your object into the field.");
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

        // Optional visual indicator at target location
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
            return;
        }

        Vector3 moveDirection = direction.normalized;
        controlledObject.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Smoothly rotate to face movement direction
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
}
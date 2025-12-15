using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("Prefabs & References")]
    [SerializeField] private GameObject objectPrefab;           // Drag your Cube prefab here
    [SerializeField] private Camera arCamera;                    // Usually auto-filled with Main Camera
    [SerializeField] private GameObject moveIndicatorPrefab;    // Optional visual marker

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private ARRaycastManager arRaycastManager;
    private GameObject controlledObject;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject currentMoveIndicator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        HandleTouchInput();

        // Always try to move if we have an object and a target
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

            // First tap: spawn the cube
            if (controlledObject == null)
            {
                SpawnObject(hitPoint);
            }

            // Every tap: set new target
            SetNewTarget(hitPoint);
        }
    }

    private void SpawnObject(Vector3 position)
    {
        if (objectPrefab == null)
        {
            Debug.LogError("No objectPrefab assigned!");
            return;
        }

        controlledObject = Instantiate(objectPrefab, position, Quaternion.identity);
        Debug.Log("Cube spawned!");
    }

    private void SetNewTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;

        // Optional indicator
        if (moveIndicatorPrefab != null)
        {
            if (currentMoveIndicator != null)
                Destroy(currentMoveIndicator);

            currentMoveIndicator = Instantiate(moveIndicatorPrefab, targetPosition, Quaternion.identity);
        }
    }

    private void MoveTowardsTarget()
    {
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

        // Rotate to face movement direction
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
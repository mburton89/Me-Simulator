using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("Prefabs & References")]
    [SerializeField] private GameObject objectPrefab;           // Drag your Cube prefab here
    [SerializeField] private Camera arCamera;                    // AR Session Origin > AR Camera
    [SerializeField] private GameObject moveIndicatorPrefab;    // Optional: visual ring/circle at target

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private ARRaycastManager arRaycastManager;
    private GameObject spawnedObject;        // The cube (or later: avatar)
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject currentMoveIndicator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;

        // Start with no target
        targetPosition = Vector3.zero;
    }

    private void Update()
    {
        HandleTouchInput();

        if (isMoving && spawnedObject != null)
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

            // First tap: Spawn the cube if it doesn't exist yet
            if (spawnedObject == null)
            {
                SpawnObject(hitPoint);
            }

            // Every tap: Set new target
            SetNewTarget(hitPoint);
        }
    }

    private void SpawnObject(Vector3 position)
    {
        if (objectPrefab == null)
        {
            Debug.LogError("ARTapToMove: No objectPrefab assigned!");
            return;
        }

        spawnedObject = Instantiate(objectPrefab, position, Quaternion.identity);
        Debug.Log("Cube spawned at first tap!");
    }

    private void SetNewTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;

        // Optional visual indicator
        if (moveIndicatorPrefab != null)
        {
            if (currentMoveIndicator != null)
                Destroy(currentMoveIndicator);

            currentMoveIndicator = Instantiate(moveIndicatorPrefab, targetPosition, Quaternion.identity);
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = targetPosition - spawnedObject.transform.position;
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

        // Move
        Vector3 moveDirection = direction.normalized;
        spawnedObject.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotate to face movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            spawnedObject.transform.rotation = Quaternion.Slerp(
                spawnedObject.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
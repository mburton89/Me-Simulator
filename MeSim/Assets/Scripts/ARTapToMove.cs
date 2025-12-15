using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rpmPlayerRoot; // Drag RPM Player here
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject moveIndicatorPrefab; // Optional

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;

    private ARRaycastManager arRaycastManager;
    private GameObject controlledObject;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject currentMoveIndicator;
    private Animator currentAnimator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        if (arCamera == null) arCamera = Camera.main;
    }

    private void Update()
    {
        // Always get the current avatar (Preview or loaded)
        if (rpmPlayerRoot != null && rpmPlayerRoot.childCount > 0)
        {
            controlledObject = rpmPlayerRoot.GetChild(0).gameObject;
            currentAnimator = controlledObject.GetComponentInChildren<Animator>();
        }

        HandleTouchInput();

        if (isMoving && controlledObject != null)
        {
            MoveTowardsTarget();
        }
        else if (currentAnimator != null)
        {
            currentAnimator.SetFloat("Speed", 0f); // Idle when stopped
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
            SetNewTarget(hits[0].pose.position);
        }
    }

    private void SetNewTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;

        if (moveIndicatorPrefab != null)
        {
            if (currentMoveIndicator != null) Destroy(currentMoveIndicator);
            currentMoveIndicator = Instantiate(moveIndicatorPrefab, targetPosition, Quaternion.identity);
        }

        if (currentAnimator != null)
        {
            currentAnimator.SetFloat("Speed", 1f); // Walk
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = targetPosition - controlledObject.transform.position;
        if (direction.magnitude <= stopDistance)
        {
            isMoving = false;
            if (currentMoveIndicator != null) Destroy(currentMoveIndicator);
            if (currentAnimator != null) currentAnimator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 moveDir = direction.normalized;
        controlledObject.transform.position += moveDir * moveSpeed * Time.deltaTime;

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        controlledObject.transform.rotation = Quaternion.Slerp(controlledObject.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
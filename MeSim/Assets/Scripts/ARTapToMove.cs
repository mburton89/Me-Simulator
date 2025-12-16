using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("Drag your cube (or any object already in the scene) here")]
    private GameObject controlledObject;

    [SerializeField] private Camera arCamera;

    [SerializeField]
    [Tooltip("Optional: a visual marker to show where you tapped")]
    private GameObject moveIndicatorPrefab;

    private ARRaycastManager arRaycastManager;
    private GameObject currentMoveIndicator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;

        if (controlledObject == null)
        {
            Debug.LogError("ARTapToMove: Please assign the controlledObject (your cube) in the Inspector!");
        }
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
        {
            foreach (var hit in hits)
            {
                if (hit.trackable is ARPlane plane && plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 hitPoint = hit.pose.position + Vector3.up * 0.02f; // tiny lift

                    if (controlledObject != null)
                    {
                        controlledObject.transform.position = hitPoint;
                    }

                    if (moveIndicatorPrefab != null)
                    {
                        if (currentMoveIndicator != null)
                            Destroy(currentMoveIndicator);
                        currentMoveIndicator = Instantiate(moveIndicatorPrefab, hitPoint, Quaternion.identity);
                    }
                    break; // stop after first valid floor hit
                }
            }
        }
    }
}
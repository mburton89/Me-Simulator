using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("UI Feedback")]
    [SerializeField]
    [Tooltip("UI Image to show placement status: Green = success, Red = failure")]
    private Image statusIndicatorImage;

    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failureColor = Color.red;

    private ARRaycastManager arRaycastManager;
    private GameObject currentMoveIndicator;

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in scene! Make sure ARSessionOrigin is present.");
        }

        if (arCamera == null)
        { 
            arCamera = Camera.main;
        }

        if (controlledObject == null)
        {
            Debug.LogError("ARTapToMove: Please assign the controlledObject (your cube) in the Inspector!");
        }

        if (statusIndicatorImage != null)
        {
            statusIndicatorImage.color = failureColor; // Start as red (no placement yet)
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

        // Perform raycast from touch position against detected planes
        bool hasHit = arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds);

        bool placementSuccessful = false;

        if (hasHit)
        {
            foreach (var hit in hits)
            {
                // Only accept horizontal upward-facing planes (i.e., floors/tables)
                if (hit.trackable is ARPlane plane && plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 hitPoint = hit.pose.position + Vector3.up * 0.02f; // Slight offset to avoid z-fighting

                    // Move the controlled object
                    if (controlledObject != null)
                    {
                        controlledObject.transform.position = hitPoint;
                    }

                    // Spawn or update visual indicator
                    if (moveIndicatorPrefab != null)
                    {
                        if (currentMoveIndicator != null)
                            Destroy(currentMoveIndicator);

                        currentMoveIndicator = Instantiate(moveIndicatorPrefab, hitPoint, Quaternion.identity);
                    }

                    placementSuccessful = true;
                    break; // Use the first valid floor hit
                }
            }
        }

        // Update UI status indicator color
        if (statusIndicatorImage != null)
        {
            statusIndicatorImage.color = placementSuccessful ? successColor : failureColor;
        }

        // Optional: Log for debugging
        // Debug.Log(placementSuccessful ? "Placed on ground successfully!" : "No valid ground detected.");
    }
}
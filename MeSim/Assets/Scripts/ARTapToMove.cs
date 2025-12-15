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
        if (arRaycastManager.Raycast(touch.position, hits, TrackableType.Planes))
        {
            Vector3 hitPoint = hits[0].pose.position;

            // Immediately move the cube to the tapped location
            if (controlledObject != null)
            {
                controlledObject.transform.position = hitPoint;
            }

            // Optional: Show a visual indicator at the tap location
            if (moveIndicatorPrefab != null)
            {
                if (currentMoveIndicator != null)
                    Destroy(currentMoveIndicator);

                currentMoveIndicator = Instantiate(moveIndicatorPrefab, hitPoint, Quaternion.identity);
            }
        }
    }
}
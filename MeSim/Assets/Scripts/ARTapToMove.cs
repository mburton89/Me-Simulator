using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    [Tooltip("UI Image to show floor status: Green = detected, Red = not detected")]
    private Image statusIndicatorImage;

    [SerializeField]
    [Tooltip("TextMeshPro text for instructional messages")]
    private TextMeshProUGUI instructionText;

    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failureColor = Color.red;

    private ARRaycastManager arRaycastManager;
    private GameObject currentMoveIndicator;

    // Current floor detection state
    private bool floorCurrentlyDetected = false;
    private Coroutine messageCoroutine;

    private const string InstructionMessage = "Point your camera at the floor to detect it.";
    private const string SuccessMessage = "Floor Found!";

    private void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in scene! Make sure ARSessionOrigin is present.");
        }

        if (arCamera == null)
            arCamera = Camera.main;

        if (controlledObject == null)
        {
            Debug.LogError("ARTapToMove: Please assign the controlledObject in the Inspector!");
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        // Start with red + instruction text
        UpdateFloorStatus(false);
    }

    private void Update()
    {
        // Always check floor status (detected or lost)
        CheckFloorStatusContinuously();

        // Handle taps separately
        HandleTouchInput();
    }

    private void CheckFloorStatusContinuously()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        bool currentlyDetected = false;

        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds))
        {
            foreach (var hit in hits)
            {
                if (hit.trackable is ARPlane plane && plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    currentlyDetected = true;
                    break;
                }
            }
        }

        // Only update UI if the detection state changed
        if (currentlyDetected != floorCurrentlyDetected)
        {
            floorCurrentlyDetected = currentlyDetected;
            UpdateFloorStatus(floorCurrentlyDetected);

            if (floorCurrentlyDetected)
            {
                ShowTemporaryMessage(SuccessMessage, 2f);
            }
            else
            {
                // Floor lost → show instruction again
                ShowPersistentMessage(InstructionMessage);
            }
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        bool hasHit = arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds);
        bool placementSuccessful = false;

        if (hasHit)
        {
            foreach (var hit in hits)
            {
                if (hit.trackable is ARPlane plane && plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 hitPoint = hit.pose.position + Vector3.up * 0.02f;

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

                    placementSuccessful = true;
                    break;
                }
            }
        }

        // Brief tap feedback (doesn't override continuous status)
        if (statusIndicatorImage != null)
        {
            statusIndicatorImage.color = placementSuccessful ? successColor : failureColor;
        }
    }

    private void UpdateFloorStatus(bool detected)
    {
        if (statusIndicatorImage != null)
        {
            statusIndicatorImage.color = detected ? successColor : failureColor;
        }
    }

    private void ShowPersistentMessage(string message)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        if (instructionText != null)
        {
            instructionText.text = message;
            instructionText.gameObject.SetActive(true);
        }
    }

    private void ShowTemporaryMessage(string message, float duration)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(TemporaryMessageRoutine(message, duration));
    }

    private IEnumerator TemporaryMessageRoutine(string message, float duration)
    {
        if (instructionText != null)
        {
            instructionText.text = message;
            instructionText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(duration);

        //if (instructionText != null && floorCurrentlyDetected) // Only hide if floor is still detected
        //{
        //    instructionText.gameObject.SetActive(false);
        //}

        messageCoroutine = null;
    }
}
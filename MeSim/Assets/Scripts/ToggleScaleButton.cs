using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this script to a GameObject (e.g., empty or the button itself).
/// Assign the target object and connect the button's OnClick event to ToggleScale().
/// Scales instantly between min and max scale.
/// </summary>
[RequireComponent(typeof(Button))]
public class ToggleScaleButton : MonoBehaviour
{
    public static ToggleScaleButton instance;

    [Header("Target")]
    [SerializeField]
    [Tooltip("The object you want to scale (e.g., your AR character or cube)")]
    private GameObject targetObject;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 maxScale = new Vector3(1.5f, 1.5f, 1.5f);

    // Tracks current state
    [HideInInspector] public bool isMaximized = true;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Optional: auto-grab button if this script is on the button
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleScale);
        }

        // Validation
        if (targetObject == null)
        {
            Debug.LogError("ToggleScaleButton: Please assign a target object in the Inspector!");
            enabled = false;
        }
    }

    /// <summary>
    /// Call this from a UI Button's OnClick event
    /// </summary>
    public void ToggleScale()
    {
        if (targetObject == null) return;

        if (isMaximized)
        {
            targetObject.transform.localScale = minScale;
        }
        else
        {
            targetObject.transform.localScale = maxScale;
        }

        // Toggle state
        isMaximized = !isMaximized;
    }

    // Optional: reset to max on start (uncomment if desired)
    // private void Start()
    // {
    //     if (targetObject != null)
    //         targetObject.transform.localScale = maxScale;
    //     isMaximized = true;
    // }
}
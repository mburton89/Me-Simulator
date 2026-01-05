using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A centralized debug logger for AR applications.
/// Captures Unity logs (errors, warnings, exceptions) and stores the most recent error message.
/// Useful for displaying on-screen debug info (e.g., "No surface detected") without spamming the console.
/// </summary>
public class ARDebugLogger : MonoBehaviour
{
    // Singleton instance
    public static ARDebugLogger Instance { get; private set; }

    [Header("UI Output (Optional)")]
    [SerializeField]
    [Tooltip("Optional UI Text to display the latest log message on screen")]
    private TextMeshProUGUI debugTextDisplay;

    [Header("Colors")]
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.green;

    // Stores the most recent error/warning message
    public string LatestMessage { get; private set; } = "Ready";

    // Optional: keep a history (limited to avoid memory growth)
    private Queue<string> messageHistory = new Queue<string>();
    private const int MaxHistory = 20;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        LatestMessage = "Debug Logger Initialized";
        UpdateDisplay();
    }

    private void OnEnable()
    {
        // Subscribe to Unity's log message receiver
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Always update latest message
        LatestMessage = logString;

        // Add to history
        messageHistory.Enqueue($"[{type}] {logString}");
        if (messageHistory.Count > MaxHistory)
            messageHistory.Dequeue();

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (debugTextDisplay != null)
        {
            debugTextDisplay.text = LatestMessage;
        }
    }

    // Public method to manually log a message (e.g., from other scripts)
    public void LogMessage(string message, bool isError = false)
    {
        if (isError)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    // Clear the latest message (e.g., when placement succeeds)
    public void Clear()
    {
        LatestMessage = "Ready";

        UpdateDisplay();
    }
}
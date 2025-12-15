using TMPro;
using UnityEngine;

public class OnScreenLogger : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField]
    [Tooltip("Drag your existing TextMeshProUGUI object here (must be child of a Canvas)")]
    private TextMeshProUGUI logText;

    [Header("Settings")]
    [SerializeField] private int maxLines = 15;
    [SerializeField] private bool clearOnStart = true;
    [SerializeField] private bool showTimestamp = true;
    [SerializeField] private bool showStackTraceOnError = false; // Set true if you want full stack trace

    private static OnScreenLogger instance;

    private void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Validate TMP reference
        if (logText == null)
        {
            Debug.LogError("OnScreenLogger: No TextMeshProUGUI assigned! Please drag your TMP text object into the 'Log Text' field.");
            enabled = false;
            return;
        }

        // Auto-capture all Unity console messages
        Application.logMessageReceived += HandleUnityLog;

        if (clearOnStart)
        {
            Clear();
        }
    }

    private void OnDestroy()
    {
        // Clean up subscription
        Application.logMessageReceived -= HandleUnityLog;
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        string prefix = "";
        if (showTimestamp)
        {
            prefix = $"<color=gray>[{Time.time:F1}s]</color> ";
        }

        string formattedMessage = prefix + type switch
        {
            LogType.Error or LogType.Exception => $"<color=red>{logString}</color>",
            LogType.Warning => $"<color=yellow>{logString}</color>",
            LogType.Assert => $"<color=magenta>{logString}</color>",
            _ => logString // LogType.Log
        };

        logText.text += formattedMessage + "\n";

        if ((type == LogType.Error || type == LogType.Exception) && showStackTraceOnError && !string.IsNullOrEmpty(stackTrace))
        {
            logText.text += $"<color=#FF8888>{stackTrace}</color>\n";
        }

        TrimLines();
    }

    // Manual logging (optional use)
    public static void Log(string message)
    {
        if (instance == null || instance.logText == null) return;
        instance.HandleUnityLog(message, "", LogType.Log);
    }

    public static void LogWarning(string message)
    {
        if (instance == null || instance.logText == null) return;
        instance.HandleUnityLog(message, "", LogType.Warning);
    }

    public static void LogError(string message)
    {
        if (instance == null || instance.logText == null) return;
        instance.HandleUnityLog(message, "", LogType.Error);
    }

    public static void Clear()
    {
        if (instance != null && instance.logText != null)
        {
            instance.logText.text = "<color=cyan>=== On-Screen Debug Log ===</color>\n";
        }
    }

    private void TrimLines()
    {
        string[] lines = logText.text.Split('\n');
        if (lines.Length > maxLines + 2) // +2 for header and safety
        {
            int startIndex = lines.Length - maxLines;
            logText.text = string.Join("\n", lines, startIndex, maxLines);
        }
    }
}
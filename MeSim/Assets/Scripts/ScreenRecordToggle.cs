using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenRecordToggle : MonoBehaviour
{
//    [Header("UI References")]
//    [SerializeField] private Button recordButton;
//    [SerializeField] private TextMeshProUGUI timerText;      // Shows "Recording: 00:15"
//    [SerializeField] private GameObject recordingIndicator; // Optional: red dot or icon

//    [Header("Settings")]
//    [SerializeField] private string recordingFileName = "MyARRecording.mp4";
//    [SerializeField] private Color recordingColor = Color.red; // For button/indicator

//    private bool isRecording = false;
//    private float recordStartTime;
//    private Color originalButtonColor;

//    private void Awake()
//    {
//        if (recordButton == null || timerText == null)
//        {
//            Debug.LogError("Assign Record Button and Timer TextMeshPro in Inspector!");
//            enabled = false;
//            return;
//        }

//        // Initial state
//        timerText.gameObject.SetActive(false);
//        if (recordingIndicator != null)
//            recordingIndicator.SetActive(false);

//        // Save original button color (if it has Image component)
//        if (recordButton.image != null)
//            originalButtonColor = recordButton.image.color;
//    }

//    private void OnEnable()
//    {
//        recordButton.onClick.AddListener(ToggleRecording);
//    }

//    private void OnDisable()
//    {
//        recordButton.onClick.RemoveListener(ToggleRecording);
//    }

//    private void Update()
//    {
//        if (isRecording)
//        {
//            float elapsed = Time.time - recordStartTime;
//            int minutes = Mathf.FloorToInt(elapsed / 60);
//            int seconds = Mathf.FloorToInt(elapsed % 60);
//            timerText.text = $"Recording: {minutes:00}:{seconds:00}";
//        }
//    }

//    public void ToggleRecording()
//    {
//#if UNITY_IOS || UNITY_ANDROID
//        if (isRecording)
//        {
//            StopRecording();
//        }
//        else
//        {
//            StartRecording();
//        }
//#else
//        Debug.LogWarning("Screen recording only supported on iOS and Android.");
//#endif
//    }

//    private void StartRecording()
//    {
//        isRecording = true;
//        recordStartTime = Time.time;

//        // Trigger native screen recording
//        new NativeShare().StartScreenRecord(recordingFileName);

//        // UI feedback
//        timerText.gameObject.SetActive(true);
//        if (recordingIndicator != null)
//            recordingIndicator.SetActive(true);

//        if (recordButton.image != null)
//            recordButton.image.color = recordingColor;

//        Debug.Log("Native screen recording started!");
//    }

//    private void StopRecording()
//    {
//        isRecording = false;

//        // UI reset
//        timerText.gameObject.SetActive(false);
//        if (recordingIndicator != null)
//            recordingIndicator.SetActive(false);

//        if (recordButton.image != null)
//            recordButton.image.color = originalButtonColor;

//        Debug.Log("Screen recording stopped (user stopped via system UI). Timer reset.");
//    }
}
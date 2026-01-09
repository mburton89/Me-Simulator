using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
public class ScreenshotManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button screenshotButton;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Objects to Hide During Screenshot")]
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private float countdownDuration = 1f;
    [SerializeField] private string filePrefix = "Screenshot_";

    private bool isCountingDown = false;
    private List<bool> previousActiveStates;

    private void Awake()
    {
        if (screenshotButton == null || countdownText == null)
        {
            Debug.LogError("Assign Button and Text in Inspector!");
            enabled = false;
            return;
        }
        countdownText.gameObject.SetActive(false);
    }

    private void OnEnable() => screenshotButton.onClick.AddListener(StartCountdown);
    private void OnDisable() => screenshotButton.onClick.RemoveListener(StartCountdown);

    public void StartCountdown()
    {
        if (isCountingDown) return;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        isCountingDown = true;
        screenshotButton.interactable = false;
        countdownText.gameObject.SetActive(true);

        // Hide UI elements
        previousActiveStates = new List<bool>();
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
            {
                previousActiveStates.Add(obj.activeSelf);
                obj.SetActive(false);
            }
            else previousActiveStates.Add(false);
        }

        // 3...2...1
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(countdownDuration);
        }

        countdownText.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = filePrefix + timestamp + ".png";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        ScreenCapture.CaptureScreenshot(fileName);

        // Small delay to ensure file is written before gallery save
        yield return new WaitForSeconds(1f);

        // Save to gallery (no namespace needed!)
        NativeGallery.SaveImageToGallery(path, "Screenshots", fileName, (success, savePath) =>
        {
            if (success)
            {
                Debug.Log($"Screenshot saved to gallery: {savePath}");
            }
            else
            {
                Debug.LogError("Failed to save screenshot to gallery.");
            }
        });

        Debug.Log("Screenshot captured and save request sent to gallery.");

        // Restore UI
        for (int i = 0; i < objectsToHide.Count; i++)
        {
            if (objectsToHide[i] != null && i < previousActiveStates.Count)
                objectsToHide[i].SetActive(previousActiveStates[i]);
        }

        screenshotButton.interactable = true;
        isCountingDown = false;
    }
}
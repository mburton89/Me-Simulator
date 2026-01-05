using System;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.QuickStart
{
    public class PersonalAvatarLoader : MonoBehaviour
    {
        private const string SAVED_AVATAR_URL_KEY = "SavedPersonalAvatarURL";

        [Header("UI")]
        [SerializeField] private Text openPersonalAvatarPanelButtonText;
        [SerializeField] private Text linkText;
        [SerializeField] private InputField avatarUrlField;
        [SerializeField] private Button openPersonalAvatarPanelButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button linkButton;
        [SerializeField] private Button loadAvatarButton;
        [SerializeField] private GameObject avatarLoading;
        [SerializeField] private GameObject personalAvatarPanel;

        [Header("Character Managers")]
        [SerializeField] private ThirdPersonLoader thirdPersonLoader;
        [SerializeField] private ThirdPersonController thirdPersonController;

        private string defaultButtonText;
        private string pendingAvatarUrl;
        private bool attemptAutoLoad = false; // Flag to trigger delayed auto-load

        [SerializeField] GameObject smallButtonContainer;
        [SerializeField] GameObject largeButtonContainer;

        [SerializeField] Button smallRPMButton;

        private void Awake()
        {
            thirdPersonLoader.OnLoadComplete += OnThirdPersonLoadComplete;
        }

        private void Start()
        {
            AnalyticsRuntimeLogger.EventLogger.LogRunQuickStartScene();

            defaultButtonText = openPersonalAvatarPanelButtonText.text;

            // Mark that we should try auto-loading after one frame
            if (PlayerPrefs.HasKey(SAVED_AVATAR_URL_KEY))
            {
                string savedUrl = PlayerPrefs.GetString(SAVED_AVATAR_URL_KEY);
                if (!string.IsNullOrEmpty(savedUrl) && Uri.TryCreate(savedUrl, UriKind.Absolute, out _))
                {
                    pendingAvatarUrl = savedUrl;
                    attemptAutoLoad = true; 
                    Debug.Log($"Queued auto-load for saved avatar: {savedUrl}");
                }
            }
        }

        private void LateUpdate()
        {
            // Perform delayed auto-load after all Start() methods have run
            if (attemptAutoLoad)
            {
                attemptAutoLoad = false;

                if (!string.IsNullOrEmpty(pendingAvatarUrl))
                {
                    LoadAvatar(pendingAvatarUrl);
                    pendingAvatarUrl = null; // Clear early to avoid re-trigger
                }
            }
        }

        private void OnEnable()
        {
            openPersonalAvatarPanelButton.onClick.AddListener(OnOpenPersonalAvatarPanel);
            smallRPMButton.onClick.AddListener(OnOpenPersonalAvatarPanel);
            closeButton.onClick.AddListener(OnCloseButton);
            linkButton.onClick.AddListener(OnLinkButton);
            loadAvatarButton.onClick.AddListener(OnLoadAvatarButton);
            avatarUrlField.onValueChanged.AddListener(OnAvatarUrlFieldValueChanged);
        }

        private void OnDisable()
        {
            openPersonalAvatarPanelButton.onClick.RemoveListener(OnOpenPersonalAvatarPanel);
            smallRPMButton.onClick.RemoveListener(OnOpenPersonalAvatarPanel);
            closeButton.onClick.RemoveListener(OnCloseButton);
            linkButton.onClick.RemoveListener(OnLinkButton);
            loadAvatarButton.onClick.RemoveListener(OnLoadAvatarButton);
            avatarUrlField.onValueChanged.RemoveListener(OnAvatarUrlFieldValueChanged);

            thirdPersonLoader.OnLoadComplete -= OnThirdPersonLoadComplete;
        }

        private void OnOpenPersonalAvatarPanel()
        {
            personalAvatarPanel.SetActive(true);
            SetActiveThirdPersonalControls(false);
            AnalyticsRuntimeLogger.EventLogger.LogLoadPersonalAvatarButton();

            if (PlayerPrefs.HasKey(SAVED_AVATAR_URL_KEY))
            {
                avatarUrlField.text = PlayerPrefs.GetString(SAVED_AVATAR_URL_KEY);
            }
        }

        private void OnCloseButton()
        {
            SetActiveThirdPersonalControls(true);
            personalAvatarPanel.SetActive(false);
        }

        private void OnLinkButton()
        {
            Application.OpenURL(linkText.text);
        }

        private void OnLoadAvatarButton()
        {
            string url = avatarUrlField.text.Trim();
            if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                LoadAvatar(url);
                AnalyticsRuntimeLogger.EventLogger.LogPersonalAvatarLoading(url);
            }
        }

        private void LoadAvatar(string url)
        {
            pendingAvatarUrl = url;

            SetActiveLoading(true, "Loading Avatar...");

            thirdPersonLoader.LoadAvatar(url);

            personalAvatarPanel.SetActive(false);
            SetActiveThirdPersonalControls(true);
        }

        private void OnThirdPersonLoadComplete()
        {
            if (thirdPersonLoader.avatar != null)
            {
                // SUCCESS
                if (!string.IsNullOrEmpty(pendingAvatarUrl))
                {
                    PlayerPrefs.SetString(SAVED_AVATAR_URL_KEY, pendingAvatarUrl);
                    PlayerPrefs.Save();
                    Debug.Log($"Avatar loaded and URL saved: {pendingAvatarUrl}");
                }

                largeButtonContainer.SetActive(false);
                smallButtonContainer.SetActive(true);
            }
            else
            {
                // FAILURE
                Debug.LogWarning("Avatar failed to load.");
                // Do not save bad URL
            }

            SetActiveLoading(false, defaultButtonText);
            pendingAvatarUrl = null;
        }

        private void OnAvatarUrlFieldValueChanged(string url)
        {
            bool isValid = !string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out _);
            loadAvatarButton.interactable = isValid;
        }

        private void SetActiveLoading(bool enable, string text)
        {
            openPersonalAvatarPanelButtonText.text = text;
            openPersonalAvatarPanelButton.interactable = !enable;
            avatarLoading.SetActive(enable);
        }

        private void SetActiveThirdPersonalControls(bool enable)
        {
            thirdPersonController.enabled = enable;
        }
    }
}
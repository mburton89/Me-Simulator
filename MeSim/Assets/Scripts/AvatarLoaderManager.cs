using ReadyPlayerMe.Core;
using UnityEngine;

public class AvatarLoaderManager : MonoBehaviour
{
    [Header("Avatar Settings")]
    [SerializeField] private string defaultAvatarUrl = "https://models.readyplayer.me/6185a4acfb622cf1cdc49348.glb"; // Sample default

    [Header("References")]
    [SerializeField] private ARTapToMove arTapToMove; // Drag your ARTapToMove script's GameObject here

    private AvatarObjectLoader avatarObjectLoader;
    private GameObject loadedAvatar;

    private void Awake()
    {
        avatarObjectLoader = new AvatarObjectLoader();
        avatarObjectLoader.OnCompleted += OnAvatarLoaded;
        avatarObjectLoader.OnFailed += OnAvatarLoadFailed;
    }

    private void Start()
    {
        // Load default avatar on start
        LoadAvatar(defaultAvatarUrl);
    }

    public void LoadAvatar(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        avatarObjectLoader.LoadAvatar(url.Trim());
    }

    private void OnAvatarLoaded(object sender, CompletionEventArgs args)
    {
        loadedAvatar = args.Avatar;

        // Position it at origin or current cube position
        loadedAvatar.transform.position = arTapToMove.CurrentObjectPosition; // We'll add this helper below
        loadedAvatar.transform.rotation = Quaternion.identity;

        // Tell ARTapToMove to switch to this new object
        arTapToMove.SwitchToNewObject(loadedAvatar);

        Debug.Log("RPM Avatar loaded and now controlled by tap-to-move!");
    }

    private void OnAvatarLoadFailed(object sender, FailureEventArgs args)
    {
        Debug.LogError("Avatar load failed: " + args.Message);
    }

    // Optional: UI button to load custom URL
    public void LoadCustomUrl(string url)
    {
        LoadAvatar(url);
    }
}
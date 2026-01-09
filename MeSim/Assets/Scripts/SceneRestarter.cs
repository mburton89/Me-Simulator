using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    // Call this method from a UI Button's OnClick event

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(RestartScene);
    }

    public void RestartScene()
    {
        // Get the currently active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        // Alternative: Use build index if you prefer
        // SceneManager.LoadScene(currentScene.buildIndex);
    }
}
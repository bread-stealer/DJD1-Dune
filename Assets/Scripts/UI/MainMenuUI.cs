using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private SceneRef gameScene;

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnValidate()
    {
    #if UNITY_EDITOR
        gameScene?.OnValidate();
    #endif
    }

    public void OnPlayPressed()
    {
        Debug.Log($"[MainMenuUI] Trying to load scene: '{gameScene.SceneName}'");

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAndLoad(gameScene.SceneName);
        else
            SceneManager.LoadScene(gameScene.SceneName);
    }
    public void OnSettingsPressed()
    {
        settingsPanel.SetActive(true);
    }

    public void OnCreditsPressed()
    {
        creditsPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClosePanel()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
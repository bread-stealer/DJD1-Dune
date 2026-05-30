using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject winPanel;

    [Header("Scenes")]
    [SerializeField] private SceneRef nextLevelScene;
    [SerializeField] private SceneRef mainMenuScene;

    private void OnValidate()
    {
        nextLevelScene?.OnValidate();
        mainMenuScene?.OnValidate();
    }

    private void Start()
    {
        // Panel starts hidden
        winPanel.SetActive(false);
    }

    public void ShowWinScreen()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Called by Next Level button
    public void OnNextLevelPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelScene.SceneName);
    }

    // Called by Main Menu button
    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene.SceneName);
    }
}

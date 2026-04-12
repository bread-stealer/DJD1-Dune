using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private PlayerHealth _playerHealth;

    private void Start()
    {
        // Panel starts hidden
        gameOverPanel.SetActive(false);

        _playerHealth = FindObjectOfType<PlayerHealth>();
        if (_playerHealth != null)
            _playerHealth.OnDeath += HandlePlayerDeath;
        else
            Debug.LogError("[GameOverUI] PlayerHealth not found in scene.", this);
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.OnDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Freeze the game
    }

    // Called by Restart button
    public void OnRestartPressed()
    {
        Time.timeScale = 1f; // Always restore before loading
        SceneManager.LoadScene(gameSceneName);
    }

    // Called by Main Menu button
    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

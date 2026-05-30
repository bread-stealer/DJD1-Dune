using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Scenes")]
    [SerializeField] private SceneRef gameScene;
    [SerializeField] private SceneRef mainMenuScene;

    [Header("Delay")]
    // How long to wait after death before freezing and showing Game Over
    [SerializeField] private float gameOverDelay = 1f;

    private PlayerHealth _playerHealth;

    private void OnValidate()
    {
        gameScene?.OnValidate();
        mainMenuScene?.OnValidate();
    }

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
        StartCoroutine(ShowGameOverDelayed());
    }

    private IEnumerator ShowGameOverDelayed()
    {
        // Wait for worm animation to finish before freezing
        yield return new WaitForSeconds(gameOverDelay);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Called by Restart button
    public void OnRestartPressed()
    {
        // Always restore time before loading
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene.SceneName);
    }

    // Called by Main Menu button
    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene.SceneName);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button firstSelectedButton;

    [Header("Scenes")]
    [SerializeField] private SceneRef gameScene;
    [SerializeField] private SceneRef mainMenuScene;

    [Header("Delay")]
    // How long to wait after death before freezing and showing Game Over
    [SerializeField] private float gameOverDelay = 1f;

    [Header("Player Reference")]
    [SerializeField] private PlayerHealth _playerHealth;

    private void OnValidate()
    {
        gameScene?.OnValidate();
        mainMenuScene?.OnValidate();
    }

    private void Start()
    {
        // Panel starts hidden
        gameOverPanel.SetActive(false);

        if (_playerHealth != null)
            _playerHealth.OnDeath += HandlePlayerDeath;
        else
            Debug.LogError("[GameOverUI] PlayerHealth reference is not assigned.", this);
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
        // Use realtime so delay works regardless of timescale
        yield return new WaitForSecondsRealtime(gameOverDelay);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        // Wait a real frame for the panel to fully enable
        yield return new WaitForSecondsRealtime(0.1f);

        // Reset to null first to force the EventSystem to re-evaluate
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
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

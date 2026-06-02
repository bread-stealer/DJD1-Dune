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
    #if UNITY_EDITOR
        gameScene?.OnValidate();
        mainMenuScene?.OnValidate();
    #endif
    }

    private void Start()
    {
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

        PostProcessingTransition.Instance?.TransitionToMenu();
        
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        // Wait a real frame for the panel to fully enable
        yield return new WaitForSecondsRealtime(0.1f);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    // Called by Restart button
    public void OnRestartPressed()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAndLoad(gameScene.SceneName);
        else
            SceneManager.LoadScene(gameScene.SceneName);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAndLoad(mainMenuScene.SceneName);
        else
            SceneManager.LoadScene(mainMenuScene.SceneName);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    public static event System.Action OnWin;

    [Header("References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button firstSelectedButton;

    [Header("Scenes")]
    [SerializeField] private SceneRef nextLevelScene;
    [SerializeField] private SceneRef mainMenuScene;

    private void OnValidate()
    {
    # if UNITY_EDITOR
        nextLevelScene?.OnValidate();
        mainMenuScene?.OnValidate();
    #endif
    }

    private void Start()
    {
        winPanel.SetActive(false);
    }

    public void ShowWinScreen()
    {
        OnWin?.Invoke();
        winPanel.SetActive(true);
        PostProcessingTransition.Instance?.TransitionToMenu();
        Time.timeScale = 0f;
        StartCoroutine(SelectFirstButton());
    }

    private IEnumerator SelectFirstButton()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    // Called by Next Level button
    public void OnNextLevelPressed()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAndLoad(nextLevelScene.SceneName);
        else
            SceneManager.LoadScene(nextLevelScene.SceneName);
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

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
        OnWin?.Invoke();
        winPanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(SelectFirstButton());
    }

    private IEnumerator SelectFirstButton()
    {
        // Wait a real frame for the panel to fully enable
        yield return new WaitForSecondsRealtime(0.1f);

        // Reset to null first to force the EventSystem to re-evaluate
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
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

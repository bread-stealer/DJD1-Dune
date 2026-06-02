using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Fade in on first load
        StartCoroutine(FadeRoutine(1f, 0f, fadeInDuration, null));
    }

    public void FadeToScene(string sceneName, System.Action onFadeComplete = null)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneName, onFadeComplete));
    }

    private IEnumerator FadeOutAndLoadRoutine(string sceneName, System.Action onFadeComplete)
    {
        // Fade to black
        yield return StartCoroutine(FadeRoutine(0f, 1f, fadeOutDuration, null));

        onFadeComplete?.Invoke();
        SceneManager.LoadScene(sceneName);

        // Wait a frame for the scene to load
        yield return null;

        // Fade back in
        yield return StartCoroutine(FadeRoutine(1f, 0f, fadeInDuration, null));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, System.Action onComplete)
    {
        fadeCanvasGroup.alpha = from;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
        onComplete?.Invoke();
    }
}

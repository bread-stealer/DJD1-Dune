using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingTransition : MonoBehaviour
{
    public static PostProcessingTransition Instance { get; private set; }

    [Header("Volumes")]
    [SerializeField] private Volume levelVolume;
    [SerializeField] private Volume menuVolume;

    [Header("Settings")]
    [SerializeField] private float transitionDuration = 1f;

    private Coroutine _activeTransition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        levelVolume.weight = 1f;
        menuVolume.weight = 0f;
    }

    public void TransitionToMenu()
    {
        Debug.Log("[PostProcessingTransition] TransitionToMenu called");
        StartTransition(1f, 0f);
    }

    public void TransitionToLevel()
    {
        StartTransition(0f, 1f);
    }

    private void StartTransition(float menuTarget, float levelTarget)
    {
        if (_activeTransition != null)
            StopCoroutine(_activeTransition);
        _activeTransition = StartCoroutine(TransitionRoutine(menuTarget, levelTarget));
    }

    private IEnumerator TransitionRoutine(float menuTarget, float levelTarget)
    {
        float elapsed = 0f;
        float menuStart = menuVolume.weight;
        float levelStart = levelVolume.weight;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionDuration;

            menuVolume.weight = Mathf.Lerp(menuStart, menuTarget, t);
            levelVolume.weight = Mathf.Lerp(levelStart, levelTarget, t);

            yield return null;
        }

        menuVolume.weight = menuTarget;
        levelVolume.weight = levelTarget;
        _activeTransition = null;
    }
}
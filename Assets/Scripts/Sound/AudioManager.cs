using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Settings")]
    [SerializeField] private float pitchVariation = 0.1f;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip gameMusicClip;
    [SerializeField] private SceneRef gameScene;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;

    private float _originalPitch;
    private Coroutine _activeFade;
    private PlayerHealth _playerHealth;

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

    private void OnValidate()
    {
    #if UNITY_EDITOR
        gameScene?.OnValidate();
    #endif
    }
    private void OnEnable()
    {
        WinUI.OnWin += HandleGameEnd;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        WinUI.OnWin -= HandleGameEnd;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        _originalPitch = sfxSource.pitch;
        Debug.Log($"[AudioManager] sfxSource: {sfxSource}, musicSource: {musicSource}, clip: {backgroundMusic}");

        if (backgroundMusic != null)
            PlayMusicWithFadeIn(backgroundMusic);
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayerDeath();
    }

    // Called every time any scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnsubscribeFromPlayerDeath();

        // Re-find PlayerHealth in the new scene
        _playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (_playerHealth != null)
            _playerHealth.OnDeath += HandleGameEnd;

        AudioClip clipToPlay = scene.name == gameScene.SceneName ? gameMusicClip : backgroundMusic;

        if (clipToPlay != null)
            PlayMusicWithFadeIn(clipToPlay);
    }

    private void UnsubscribeFromPlayerDeath()
    {
        if (_playerHealth != null)
            _playerHealth.OnDeath -= HandleGameEnd;

        _playerHealth = null;
    }

    private void HandleGameEnd()
    {
        FadeOutMusic();
    }

    public void PlaySFX(AudioClip clip, float volume, bool randomPitch = true)
    {
        if (clip == null) return;

        sfxSource.pitch = randomPitch
            ? _originalPitch + Random.Range(-pitchVariation, pitchVariation)
            : _originalPitch;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayMusicWithFadeIn(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = 0f;
        musicSource.Play();

        StartFade(fadeInDuration, musicVolume);
    }

    public void FadeOutMusic(System.Action onComplete = null)
    {
        StartFade(fadeOutDuration, 0f, onComplete);
    }

    // Single entry point for any UI
    public void FadeOutAndLoad(string sceneName)
    {
        FadeOutMusic();

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.FadeToScene(sceneName);
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    private void StartFade(float duration, float targetVolume, System.Action onComplete = null)
    {
        if (_activeFade != null)
            StopCoroutine(_activeFade);

        _activeFade = StartCoroutine(FadeRoutine(duration, targetVolume, onComplete));
    }

    private IEnumerator FadeRoutine(float duration, float targetVolume, System.Action onComplete)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        _activeFade = null;
        onComplete?.Invoke();
    }
}
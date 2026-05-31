using System.Collections;
using UnityEngine;

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
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;

    [Header("Player Reference")]
    [SerializeField] private PlayerHealth playerHealth;

    private float _originalPitch;
    private Coroutine _activeFade;

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

    private void OnEnable()
    {
        WinUI.OnWin += HandleGameEnd;
    }

    private void OnDisable()
    {
        WinUI.OnWin -= HandleGameEnd;
    }

    private void Start()
    {
        _originalPitch = sfxSource.pitch;

        if (playerHealth != null)
            playerHealth.OnDeath += HandleGameEnd;

        if (backgroundMusic != null)
            PlayMusicWithFadeIn(backgroundMusic);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= HandleGameEnd;
    }

    private void HandleGameEnd()
    {
        FadeOutMusic();
    }

    public void PlaySFX(AudioClip clip, bool randomPitch = true)
    {
        if (clip == null) return;

        sfxSource.pitch = randomPitch
            ? _originalPitch + Random.Range(-pitchVariation, pitchVariation)
            : _originalPitch;

        sfxSource.PlayOneShot(clip);
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

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
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
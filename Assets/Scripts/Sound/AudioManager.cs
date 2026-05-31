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

    private float _originalPitch;

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
        _originalPitch = sfxSource.pitch;

        if (backgroundMusic != null)
            PlayMusic(backgroundMusic);
    }

    // Play a one-shot SFX with optional pitch variation
    public void PlaySFX(AudioClip clip, bool randomPitch = true)
    {
        if (clip == null) return;

        sfxSource.pitch = randomPitch
            ? _originalPitch + Random.Range(-pitchVariation, pitchVariation)
            : _originalPitch;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
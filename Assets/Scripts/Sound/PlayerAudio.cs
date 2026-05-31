using UnityEngine;


// Hooks into existing events to trigger audio
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerShield))]
public class PlayerAudio : MonoBehaviour
{
    [Header("SFX Clips")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip shieldOnClip;
    [SerializeField] private AudioClip shieldOffClip;
    [SerializeField] private AudioClip shieldBlockClip;
    [SerializeField] private AudioClip shieldBrokenClip;

    private PlayerHealth _health;
    private PlayerShield _shield;
    private PlayerController _controller;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _shield = GetComponent<PlayerShield>();
        _controller = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDeath += HandleDeath;
        _shield.OnShieldActivated += HandleShieldOn;
        _shield.OnShieldDeactivated += HandleShieldOff;
        _shield.OnShieldBlocked += HandleShieldBlock;
        _shield.OnShieldBroken += HandleShieldBroken;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnDeath -= HandleDeath;
        _shield.OnShieldActivated -= HandleShieldOn;
        _shield.OnShieldDeactivated -= HandleShieldOff;
        _shield.OnShieldBlocked -= HandleShieldBlock;
        _shield.OnShieldBroken -= HandleShieldBroken;
    }

    // Called by Animation Event on the attack animation
    public void PlayAttackSFX()
    {
        AudioManager.Instance?.PlaySFX(attackClip);
    }

    // Called by PlayerController when landing
    public void PlayLandSFX()
    {
        AudioManager.Instance?.PlaySFX(landClip, randomPitch: false);
    }

    public void PlayJumpSFX()
    {
        AudioManager.Instance?.PlaySFX(jumpClip);
    }

    private void HandleHealthChanged(float current, float max)
    {
        AudioManager.Instance?.PlaySFX(hitClip);
    }

    private void HandleDeath()
    {
        AudioManager.Instance?.PlaySFX(deathClip, randomPitch: false);
    }

    private void HandleShieldOn()
    {
        AudioManager.Instance?.PlaySFX(shieldOnClip, randomPitch: false);
    }

    private void HandleShieldOff()
    {
        AudioManager.Instance?.PlaySFX(shieldOffClip, randomPitch: false);
    }

    private void HandleShieldBlock()
    {
        AudioManager.Instance?.PlaySFX(shieldBlockClip);
    }

    private void HandleShieldBroken()
    {
        AudioManager.Instance?.PlaySFX(shieldBrokenClip, randomPitch: false);
    }
}

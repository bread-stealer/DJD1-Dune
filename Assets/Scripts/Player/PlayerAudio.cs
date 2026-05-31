using UnityEngine;

// Hooks into existing events to trigger audio
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerShield))]
public class PlayerAudio : MonoBehaviour
{
    [Header("SFX Clips")]
    [SerializeField] private SFXClip attackClip;
    [SerializeField] private SFXClip hitClip;
    [SerializeField] private SFXClip deathClip;
    [SerializeField] private SFXClip jumpClip;
    [SerializeField] private SFXClip landClip;
    [SerializeField] private SFXClip shieldOnClip;
    [SerializeField] private SFXClip shieldOffClip;
    [SerializeField] private SFXClip shieldBlockClip;
    [SerializeField] private SFXClip shieldBrokenClip;

    [Header("Footstep Clips")]
    [SerializeField] private SFXClip sandFootstepClip;
    [SerializeField] private SFXClip rockFootstepClip;

    [Header("Footstep Detection")]
    [SerializeField] private LayerMask sandLayer;
    [SerializeField] private LayerMask rockLayer;
    [SerializeField] private float groundRayDistance = 0.3f;

    [Header("References")]
    [SerializeField] private AnimEventBridge animEventBridge;

    private PlayerHealth _health;
    private PlayerShield _shield;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _shield = GetComponent<PlayerShield>();
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDeath += HandleDeath;
        _shield.OnShieldActivated += HandleShieldOn;
        _shield.OnShieldDeactivated += HandleShieldOff;
        _shield.OnShieldBlocked += HandleShieldBlock;
        _shield.OnShieldBroken += HandleShieldBroken;

        if (animEventBridge != null)
            animEventBridge.OnFootstepEvent += HandleFootstep;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnDeath -= HandleDeath;
        _shield.OnShieldActivated -= HandleShieldOn;
        _shield.OnShieldDeactivated -= HandleShieldOff;
        _shield.OnShieldBlocked -= HandleShieldBlock;
        _shield.OnShieldBroken -= HandleShieldBroken;

        if (animEventBridge != null)
            animEventBridge.OnFootstepEvent -= HandleFootstep;
    }

    private void HandleFootstep()
    {
        RaycastHit2D sandHit = Physics2D.Raycast(transform.position, Vector2.down, groundRayDistance, sandLayer);
        if (sandHit.collider != null)
        {
            AudioManager.Instance?.PlaySFX(sandFootstepClip.clip, sandFootstepClip.volume);
            return;
        }

        RaycastHit2D rockHit = Physics2D.Raycast(transform.position, Vector2.down, groundRayDistance, rockLayer);
        if (rockHit.collider != null)
            AudioManager.Instance?.PlaySFX(rockFootstepClip.clip, rockFootstepClip.volume);
    }

    // Called by Animation Event on the attack animation
    public void PlayAttackSFX()
    {
        AudioManager.Instance?.PlaySFX(attackClip.clip, attackClip.volume);
    }

    // Called by PlayerController when landing
    public void PlayLandSFX()
    {
        AudioManager.Instance?.PlaySFX(landClip.clip, landClip.volume, randomPitch: false);
    }

    // Called by PlayerController when jumping
    public void PlayJumpSFX()
    {
        AudioManager.Instance?.PlaySFX(jumpClip.clip, jumpClip.volume);
    }

    private void HandleHealthChanged(float current, float max)
    {
        AudioManager.Instance?.PlaySFX(hitClip.clip, hitClip.volume);
    }

    private void HandleDeath()
    {
        AudioManager.Instance?.PlaySFX(deathClip.clip, deathClip.volume, randomPitch: false);
    }

    private void HandleShieldOn()
    {
        AudioManager.Instance?.PlaySFX(shieldOnClip.clip, shieldOnClip.volume, randomPitch: false);
    }

    private void HandleShieldOff()
    {
        AudioManager.Instance?.PlaySFX(shieldOffClip.clip, shieldOffClip.volume, randomPitch: false);
    }

    private void HandleShieldBlock()
    {
        AudioManager.Instance?.PlaySFX(shieldBlockClip.clip, shieldBlockClip.volume);
    }

    private void HandleShieldBroken()
    {
        AudioManager.Instance?.PlaySFX(shieldBrokenClip.clip, shieldBrokenClip.volume, randomPitch: false);
    }
}

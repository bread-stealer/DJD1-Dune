using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerShield : MonoBehaviour
{
    [Header("Shield Sprite")]
    [SerializeField] private SpriteRenderer shieldSprite;
    [SerializeField] private Color shieldColor = new Color(0.5f, 0.8f, 1f, 0.4f);
    
    [Header("Shield Settings")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float immunityWindowDuration = 0.3f;
    [SerializeField] private float cooldownDuration = 3f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem activationParticles;
    [SerializeField] private ParticleSystem auraParticles;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    [Header("Hit Flash Settings")]
    [SerializeField] private Color hitFlashColor = new Color(0.5f, 0.8f, 1f, 1f);
    [SerializeField] private float hitFlashDuration = 0.1f;

    public bool IsShieldActive { get; private set; }
    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public bool IsOnCooldown { get; private set; }

    // Events for other systems (UI, audio, animation)
    public event Action OnShieldActivated;
    public event Action OnShieldDeactivated;
    public event Action OnShieldBroken;
    public event Action OnShieldBlocked;
    public event Action OnShieldRecharged;

    private bool _isImmune;
    private Color _originalColor;
    private Coroutine _flashCoroutine;

    private void Start()
    {
        CurrentStamina = maxStamina;

        if (playerSpriteRenderer != null)
            _originalColor = playerSpriteRenderer.color;

        if (shieldSprite != null)
        {
            shieldSprite.color = shieldColor;
            shieldSprite.enabled = false;
        }
    }

    private void Update()
    {
        HandleToggleInput();
        DrainStamina();
    }

    private void OnDisable()
    {
        // color is always reset if object is disabled mid-flash
        if (playerSpriteRenderer != null)
            playerSpriteRenderer.color = _originalColor;
    }

    private void HandleToggleInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (IsShieldActive)
            DeactivateShield();
        else if (!IsOnCooldown)
            ActivateShield();
    }

    private void ActivateShield()
    {
        if (CurrentStamina <= 0 || IsOnCooldown) return;

        IsShieldActive = true;
        StartCoroutine(ImmunityWindowRoutine());
        PlayParticles(activationParticles);
        PlayAura(true);
        if (shieldSprite != null)
            shieldSprite.enabled = true;
        OnShieldActivated?.Invoke();
    }

    private void DeactivateShield()
    {
        IsShieldActive = false;
        _isImmune = false;
        PlayAura(false);
        if (shieldSprite != null)
            shieldSprite.enabled = false;
        OnShieldDeactivated?.Invoke();
    }

    private void DrainStamina()
    {
        if (!IsShieldActive) return;

        CurrentStamina -= staminaDrainRate * Time.deltaTime;

        if (CurrentStamina <= 0)
        {
            CurrentStamina = 0;
            DeactivateShield();
            StartCoroutine(CooldownRoutine());
            OnShieldBroken?.Invoke();
        }
    }

    // Returns true if the shield fully blocked the attack
    public bool TryBlock(AttackData attack)
    {
        if (!IsShieldActive || attack.IsShieldPenetrating) return false;
        if (!_isImmune) OnBlockedHit();
        return true;
    }

    private void OnBlockedHit()
    {
        PlayParticles(hitParticles);

        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(HitFlashRoutine());

        OnShieldBlocked?.Invoke();
    }

    private void PlayParticles(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();
    }

    private void PlayAura(bool enable)
    {
        if (auraParticles == null) return;
        if (enable) auraParticles.Play();
        else auraParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private IEnumerator ImmunityWindowRoutine()
    {
        _isImmune = true;
        yield return new WaitForSeconds(immunityWindowDuration);
        _isImmune = false;
    }

    private IEnumerator CooldownRoutine()
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        CurrentStamina = maxStamina;
        IsOnCooldown = false;
        OnShieldRecharged?.Invoke();
    }

    private IEnumerator HitFlashRoutine()
    {
        playerSpriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        playerSpriteRenderer.color = _originalColor;
    }
}

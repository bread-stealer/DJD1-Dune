using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerShield playerShield;
    [SerializeField] private RectTransform fillBar;
    [SerializeField] private RectTransform ghostBar;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float ghostDelay = 0.6f;
    [SerializeField] private float ghostDrainSpeed = 2f;

    [Header("Cooldown Pulse")]
    [SerializeField] private Color cooldownColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private float pulseSpeed = 3f;

    private float _fullWidth;
    private float _targetWidth;
    private float _ghostWidth;
    private Image _fillImage;
    private Image _ghostImage;
    private Color _originalFillColor;
    private Color _originalGhostColor;
    private bool _isPulsing;
    private Coroutine _ghostDelayCoroutine;

    private void Start()
    {
        if (playerShield == null)
        {
            Debug.LogError("[ShieldBarUI] PlayerShield reference is missing.", this);
            return;
        }

        _fillImage = fillBar.GetComponent<Image>();
        _ghostImage = ghostBar.GetComponent<Image>();
        _originalFillColor = _fillImage.color;
        _originalGhostColor = _ghostImage.color;

        _fullWidth = fillBar.sizeDelta.x;
        _targetWidth = _fullWidth;
        _ghostWidth = _fullWidth;

        playerShield.OnShieldBroken += HandleShieldBroken;
        playerShield.OnShieldRecharged += HandleShieldRecharged;
        playerShield.OnShieldActivated += HandleShieldActivated;
        playerShield.OnShieldDeactivated += HandleShieldDeactivated;
    }

    private void OnDestroy()
    {
        if (playerShield == null) return;

        playerShield.OnShieldBroken -= HandleShieldBroken;
        playerShield.OnShieldRecharged -= HandleShieldRecharged;
        playerShield.OnShieldActivated -= HandleShieldActivated;
        playerShield.OnShieldDeactivated -= HandleShieldDeactivated;
    }

    private void Update()
    {
        SyncFillFromShield();
        UpdateFillBar();
        UpdateGhostBar();
        UpdateCooldownPulse();
    }

    // Poll stamina every frame while not on cooldown
    private void SyncFillFromShield()
    {
        if (playerShield.IsOnCooldown) return;
        _targetWidth = (playerShield.CurrentStamina / playerShield.MaxStamina) * _fullWidth;
    }

    private void UpdateFillBar()
    {
        float newWidth = Mathf.Lerp(fillBar.sizeDelta.x, _targetWidth, smoothSpeed * Time.deltaTime);
        fillBar.sizeDelta = new Vector2(newWidth, fillBar.sizeDelta.y);
    }

    private void UpdateGhostBar()
    {
        if (_ghostWidth > _targetWidth)
        {
            _ghostWidth = Mathf.Lerp(_ghostWidth, _targetWidth, ghostDrainSpeed * Time.deltaTime);
            ghostBar.sizeDelta = new Vector2(_ghostWidth, ghostBar.sizeDelta.y);
        }
        else if (_ghostWidth < _targetWidth)
        {
            _ghostWidth = Mathf.Lerp(_ghostWidth, _targetWidth, smoothSpeed * Time.deltaTime);
            ghostBar.sizeDelta = new Vector2(_ghostWidth, ghostBar.sizeDelta.y);
        }
    }

    private void UpdateCooldownPulse()
    {
        if (!_isPulsing) return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        _fillImage.color = Color.Lerp(cooldownColor, _originalFillColor, pulse);
        _ghostImage.color = Color.Lerp(cooldownColor, _originalGhostColor, pulse);
    }

    private void HandleShieldBroken()
    {
        _targetWidth = 0f;
        _isPulsing = true;
        RestartGhostDelay();
    }

    private void HandleShieldRecharged()
    {
        _targetWidth = _fullWidth;
        _isPulsing = false;
        _fillImage.color = _originalFillColor;
        _ghostImage.color = _originalGhostColor;
        RestartGhostDelay();
    }

    private void HandleShieldActivated()
    {
        RestartGhostDelay();
    }

    private void HandleShieldDeactivated()
    {
        RestartGhostDelay();
    }

    private void RestartGhostDelay()
    {
        if (_ghostDelayCoroutine != null)
            StopCoroutine(_ghostDelayCoroutine);
        _ghostDelayCoroutine = StartCoroutine(GhostDelayRoutine());
    }

    private IEnumerator GhostDelayRoutine()
    {
        yield return new WaitForSeconds(ghostDelay);
    }
}
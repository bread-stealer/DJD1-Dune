using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerHitFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private float flashDuration = 0.12f;

    private PlayerHealth _health;
    private Color _originalColor;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();

        if (spriteRenderer != null)
            _originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;

        // Always restore color if disabled mid-flash
        if (spriteRenderer != null)
            spriteRenderer.color = _originalColor;
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = _originalColor;
    }
}
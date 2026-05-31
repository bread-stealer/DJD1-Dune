using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private RectTransform fillBar;
    [SerializeField] private RectTransform ghostBar;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float ghostDelay = 0.6f;
    [SerializeField] private float ghostDrainSpeed = 2f;

    private float _fullWidth;
    private float _targetWidth;
    private float _ghostWidth;
    private Coroutine _ghostDelayCoroutine;

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("[HealthBarUI] PlayerHealth reference is missing.", this);
            return;
        }

        _fullWidth = fillBar.sizeDelta.x;
        _targetWidth = _fullWidth;
        _ghostWidth = _fullWidth;

        playerHealth.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    private void Update()
    {
        // Smooth fill bar toward target
        float newFillWidth = Mathf.Lerp(fillBar.sizeDelta.x, _targetWidth, smoothSpeed * Time.deltaTime);
        fillBar.sizeDelta = new Vector2(newFillWidth, fillBar.sizeDelta.y);

        // Ghost bar drains after delay
        if (_ghostWidth > _targetWidth)
        {
            _ghostWidth = Mathf.Lerp(_ghostWidth, _targetWidth, ghostDrainSpeed * Time.deltaTime);
            ghostBar.sizeDelta = new Vector2(_ghostWidth, ghostBar.sizeDelta.y);
        }
    }

    private void HandleHealthChanged(float current, float max)
    {
        _targetWidth = (current / max) * _fullWidth;

        if (_ghostDelayCoroutine != null)
            StopCoroutine(_ghostDelayCoroutine);
        _ghostDelayCoroutine = StartCoroutine(GhostDelayRoutine());
    }

    private IEnumerator GhostDelayRoutine()
    {
        yield return new WaitForSeconds(ghostDelay);
    }
}

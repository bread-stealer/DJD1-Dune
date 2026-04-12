using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillBar;
    [SerializeField] private Image ghostBar;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float ghostDelay = 0.6f;
    [SerializeField] private float ghostDrainSpeed = 2f;

    private float _targetFill;
    private float _ghostFill;
    private Coroutine _ghostDelayCoroutine;

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("[HealthBarUI] PlayerHealth reference is missing.", this);
            return;
        }

        // Initialise both bars to full
        _targetFill = 1f;
        _ghostFill = 1f;
        fillBar.fillAmount = 1f;
        ghostBar.fillAmount = 1f;

        playerHealth.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    private void Update()
    {
        // Smooth the fill bar toward target
        fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, _targetFill, smoothSpeed * Time.deltaTime);

        // Ghost bar drains after delay
        if (ghostBar.fillAmount > _targetFill)
            ghostBar.fillAmount = Mathf.Lerp(ghostBar.fillAmount, _targetFill, ghostDrainSpeed * Time.deltaTime);
    }

    private void HandleHealthChanged(float current, float max)
    {
        _targetFill = current / max;

        // Restart ghost delay on each hit
        if (_ghostDelayCoroutine != null)
            StopCoroutine(_ghostDelayCoroutine);
        _ghostDelayCoroutine = StartCoroutine(GhostDelayRoutine());
    }

    private IEnumerator GhostDelayRoutine()
    {
        // Ghost bar holds still for a moment before draining
        yield return new WaitForSeconds(ghostDelay);
        // Update() handles the actual drain
    }
}

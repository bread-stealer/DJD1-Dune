using UnityEngine;
using UnityEngine.UI;

public class WaterGaugeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWater playerWater;
    [SerializeField] private Image gaugeFill;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color dehydratedColor = new Color(1f, 0.3f, 0.1f);

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;

    private float _targetFill;

    private void Start()
    {
        if (playerWater == null)
        {
            Debug.LogError("[WaterGaugeUI] PlayerWater reference is missing.", this);
            return;
        }

        _targetFill = 1f;
        gaugeFill.fillAmount = 1f;
        gaugeFill.color = normalColor;

        playerWater.OnWaterChanged += HandleWaterChanged;
        playerWater.OnDehydrated += HandleDehydrated;
        playerWater.OnWaterRestored += HandleWaterRestored;
    }

    private void OnDestroy()
    {
        if (playerWater == null) return;
        playerWater.OnWaterChanged -= HandleWaterChanged;
        playerWater.OnDehydrated -= HandleDehydrated;
        playerWater.OnWaterRestored -= HandleWaterRestored;
    }

    private void Update()
    {
        // Smooth the gauge toward target
        gaugeFill.fillAmount = Mathf.Lerp(gaugeFill.fillAmount, _targetFill, smoothSpeed * Time.deltaTime);
    }

    private void HandleWaterChanged(float current, float max)
    {
        _targetFill = current / max;
    }

    private void HandleDehydrated()
    {
        // Flash red to signal danger
        gaugeFill.color = dehydratedColor;
    }

    private void HandleWaterRestored()
    {
        gaugeFill.color = normalColor;
    }
}

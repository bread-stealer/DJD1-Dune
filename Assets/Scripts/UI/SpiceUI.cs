using System;
using UnityEngine;
using TMPro;

public class SpiceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI spiceText;

    private void Start()
    {
        if (SpiceManager.Instance == null)
        {
            Debug.LogError("[SpiceUI] SpiceManager not found in scene.", this);
            return;
        }

        // Initialise display
        UpdateDisplay(0);
        SpiceManager.Instance.OnSpiceChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        if (SpiceManager.Instance != null)
            SpiceManager.Instance.OnSpiceChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int total)
    {
        if (spiceText != null)
            spiceText.text = $"Spice: {total}";
    }
}

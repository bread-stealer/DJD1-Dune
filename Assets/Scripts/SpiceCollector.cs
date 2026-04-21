using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class SpiceCollector : MonoBehaviour
{
    [Header("Spice Settings")]
    [SerializeField] private int spiceAmount = 1;
     // seconds to hold F
    [SerializeField] private float harvestDuration = 2f;

    [Header("Interaction Prompt")]
    // world space UI prompt
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TextMeshPro promptText;

    [Header("Progress")]
    [SerializeField] private SpriteRenderer progressIndicator;

    private bool _playerInRange;
    private bool _isHarvesting;
    private bool _isHarvested;
    private float _harvestProgress;
    private PlayerController _playerController;

    private void Awake()
    {
        // Collider must be a trigger for range detection
        GetComponent<Collider2D>().isTrigger = true;

        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void Update()
    {
        if (_isHarvested || !_playerInRange) return;

        HandleHarvestInput();
    }

    private void HandleHarvestInput()
    {
        if (Input.GetKey(KeyCode.F))
        {
            _isHarvesting = true;
            _harvestProgress += Time.deltaTime;

            UpdatePrompt();

            if (_harvestProgress >= harvestDuration)
                CompleteHarvest();
        }
        else
        {
            // Reset progress if player releases F
            if (_isHarvesting)
            {
                _isHarvesting = false;
                _harvestProgress = 0f;
                UpdatePrompt();
            }
        }
    }

    private void UpdatePrompt()
    {
        if (promptText == null) return;

        if (_isHarvesting)
        {
            float percent = Mathf.FloorToInt((_harvestProgress / harvestDuration) * 100f);
            promptText.text = $"Harvesting... {percent}%";
        }
        else
        {
            promptText.text = "F to Harvest";
        }
    }

    private void CompleteHarvest()
    {
        _isHarvested = true;
        _isHarvesting = false;

        if (SpiceManager.Instance != null)
            SpiceManager.Instance.AddSpice(spiceAmount);

        // Hide prompt and disable collector
        if (promptObject != null)
            promptObject.SetActive(false);

        // Visual feedback > grey out the collector
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(0.4f, 0.4f, 0.4f);

        Debug.Log($"[SpiceCollector] Harvested {spiceAmount} spice!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isHarvested) return;
        if (!other.CompareTag("Player")) return;

        _playerInRange = true;

        if (promptObject != null)
            promptObject.SetActive(true);

        if (promptText != null)
            promptText.text = "F to Harvest";
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInRange = false;
        _isHarvesting = false;
        _harvestProgress = 0f;

        if (promptObject != null)
            promptObject.SetActive(false);
    }
}

using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class SpiceExtractor : MonoBehaviour
{
    [Header("Harvesting Settings")]
    [SerializeField] private int spiceAmount = 10;
    [SerializeField] private float harvestDuration = 3f;
    [SerializeField] private KeyCode harvestKey = KeyCode.F;

    [Header("Prompt")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TextMeshPro promptText;
    [SerializeField] private string harvestMessage = "Hold F to Harvest";
    [SerializeField] private string depletedMessage = "Depleted";

    private bool _playerInRange;
    private bool _isDepleted;
    private float _harvestProgress; // 0 to 1

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInRange || _isDepleted) return;

        if (Input.GetKey(harvestKey))
        {
            _harvestProgress += Time.deltaTime / harvestDuration;
            _harvestProgress = Mathf.Clamp01(_harvestProgress);

            UpdateProgressPrompt();

            if (_harvestProgress >= 1f)
                CompleteHarvest();
        }
        else if (Input.GetKeyUp(harvestKey))
        {
            // Reset progress if player lets go early
            _harvestProgress = 0f;
            if (promptText != null)
                promptText.text = harvestMessage;
        }
    }

    private void UpdateProgressPrompt()
    {
        if (promptText == null) return;
        int percent = Mathf.RoundToInt(_harvestProgress * 100f);
        promptText.text = $"Harvesting... {percent}%";
    }

    private void CompleteHarvest()
    {
        if (SpiceManager.Instance == null)
        {
            Debug.LogError("[SpiceExtractor] SpiceManager not found in scene.", this);
            return;
        }

        SpiceManager.Instance.AddSpice(spiceAmount);
        _isDepleted = true;

        if (promptText != null)
            promptText.text = depletedMessage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInRange = true;

        if (promptObject != null)
            promptObject.SetActive(true);

        if (promptText != null)
            promptText.text = _isDepleted ? depletedMessage : harvestMessage;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInRange = false;
        _harvestProgress = 0f;

        if (promptObject != null)
            promptObject.SetActive(false);
    }
}
using UnityEngine;
using TMPro;

// Replaces both SpiceCollector and SpiceExtractor
// Configure via Inspector to match either behaviour
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
    [SerializeField] private string idleMessage = "Hold F to Harvest";
    [SerializeField] private string depletedMessage = "Depleted";

    [Header("On Depleted")]
    // Optional: grey out the sprite when depleted (SpiceCollector behaviour)
    [SerializeField] private bool greyOutOnDepleted = false;
    [SerializeField] private SpriteRenderer spriteToGreyOut;

    private bool _playerInRange;
    private bool _isDepleted;
    private bool _isHarvesting;
    private float _harvestProgress;

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
            _isHarvesting = true;
            _harvestProgress += Time.deltaTime / harvestDuration;
            _harvestProgress = Mathf.Clamp01(_harvestProgress);

            UpdateProgressPrompt();

            if (_harvestProgress >= 1f)
                CompleteHarvest();
        }
        else
        {
            // Reset progress if player releases the key early
            if (_isHarvesting)
            {
                _isHarvesting = false;
                _harvestProgress = 0f;
                SetPromptText(idleMessage);
            }
        }
    }

    private void UpdateProgressPrompt()
    {
        int percent = Mathf.RoundToInt(_harvestProgress * 100f);
        SetPromptText($"Harvesting... {percent}%");
    }

    private void CompleteHarvest()
    {
        if (SpiceManager.Instance == null)
        {
            Debug.LogError("[SpiceSource] SpiceManager not found in scene.", this);
            return;
        }

        SpiceManager.Instance.AddSpice(spiceAmount);
        _isDepleted = true;
        _isHarvesting = false;

        SetPromptText(depletedMessage);

        if (greyOutOnDepleted && spriteToGreyOut != null)
            spriteToGreyOut.color = new Color(0.4f, 0.4f, 0.4f);
    }

    private void SetPromptText(string message)
    {
        if (promptText != null)
            promptText.text = message;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInRange = true;

        if (promptObject != null)
            promptObject.SetActive(true);

        SetPromptText(_isDepleted ? depletedMessage : idleMessage);
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
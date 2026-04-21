using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class WinDoor : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TextMeshPro promptText;
    [SerializeField] private string lockedMessage = "Collect Spice First";
    [SerializeField] private string openMessage = "Enter";

    private void Awake()
    {
        // Must be a trigger > not a physics collider
        GetComponent<Collider2D>().isTrigger = true;

        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (SpiceManager.Instance == null) return;

        // Only win if player has collected spice
        if (SpiceManager.Instance.TotalSpice <= 0) return;

        WinUI winUI = FindObjectOfType<WinUI>();
        if (winUI != null)
            winUI.ShowWinScreen();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (promptObject != null)
            promptObject.SetActive(true);

        if (promptText == null) return;

        // Update prompt text based on whether player has spice
        bool hasSpice = SpiceManager.Instance != null && SpiceManager.Instance.TotalSpice > 0;
        promptText.text = hasSpice ? openMessage : lockedMessage;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (promptObject != null)
            promptObject.SetActive(false);
    }
}
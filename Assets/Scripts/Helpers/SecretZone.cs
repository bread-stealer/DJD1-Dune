using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap secretTilemap;
    [SerializeField] private TilemapRenderer secretTilemapRenderer;

    [Header("Sorting Layers")]
    [SortingLayerSelector] [SerializeField] private int outsideLayerID;
    [SortingLayerSelector] [SerializeField] private int insideLayerID;

    [Header("Colors")]
    [SerializeField] private Color outsideColor = Color.white;
    [SerializeField] private Color insideColor = new Color(0.914f, 0.914f, 0.914f, 1f);

    private void Start()
    {
        ApplyState(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ApplyState(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ApplyState(false);
    }

    private void ApplyState(bool playerInside)
    {
        if (secretTilemap != null)
            secretTilemap.color = playerInside ? insideColor : outsideColor;

        if (secretTilemapRenderer != null)
            secretTilemapRenderer.sortingLayerID = playerInside ? insideLayerID : outsideLayerID;
    }
}

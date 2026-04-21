using UnityEngine;

// Singleton > spawn damage numbers from anywhere
public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    [SerializeField] private DamageNumber damageNumberPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Spawn(float damage, bool isHeavy, Vector3 position)
    {
        if (damageNumberPrefab == null)
        {
            Debug.LogError("[DamageNumberSpawner] Prefab not assigned.", this);
            return;
        }

        // Slight random X offset so numbers don't stack on top of each other
        Vector3 spawnPosition = position + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0f);
        DamageNumber number = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);
        number.Setup(damage, isHeavy);
    }
}

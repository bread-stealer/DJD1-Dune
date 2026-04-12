using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterCollectable : MonoBehaviour
{
    [Header("Water Settings")]
    [SerializeField] private float waterAmount = 25f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem collectParticles;

    private void Awake()
    {
        // Collider must be a trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerWater playerWater = other.GetComponent<PlayerWater>();
        if (playerWater == null) return;

        // Don't collect if already full
        if (playerWater.CurrentWater >= playerWater.MaxWater) return;

        playerWater.Replenish(waterAmount);
        PlayCollectEffect();
        Destroy(gameObject);
    }

    private void PlayCollectEffect()
    {
        if (collectParticles == null) return;

        // Detach so particles survive the Destroy call
        collectParticles.transform.SetParent(null);
        collectParticles.Play();
        Destroy(collectParticles.gameObject, collectParticles.main.duration);
    }
}
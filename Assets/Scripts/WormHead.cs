using UnityEngine;

public class WormHead : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        // Sandworm always kills instantly — shield never blocks it
        var wormAttack = new AttackData(
            damage: health.MaxHealth,
            isShieldPenetrating: true
        );
        player.TakeDamage(wormAttack);
        Debug.Log("[WormHead] Player devoured!");
    }
}

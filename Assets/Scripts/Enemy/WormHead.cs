using UnityEngine;

public class WormHead : MonoBehaviour
{
    private bool _canDamage = false;

    public void SetActive(bool active)
    {
        _canDamage = active;
        Debug.Log($"[WormHead] canDamage set to {active}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[WormHead] Trigger hit by: {other.gameObject.name} on layer {other.gameObject.layer}");
        if (!_canDamage) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        var wormAttack = new AttackData(
            damage: health.MaxHealth,
            isShieldPenetrating: true
        );
        player.TakeDamage(wormAttack);
        Debug.Log("[WormHead] Player devoured!");
    }
}

using UnityEngine;

public class WormHead : MonoBehaviour
{
    private bool _canDamage = false;

    public void SetActive(bool active)
    {
        _canDamage = active;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canDamage) return;

        // Kill player
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                player.TakeDamage(new AttackData(health.MaxHealth, isShieldPenetrating: true));
            }
            return;
        }

        // Kill Harkonnen
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
            enemy.TakeDamage(float.MaxValue);
    }
}

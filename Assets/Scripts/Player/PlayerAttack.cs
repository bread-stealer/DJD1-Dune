using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D weaponHitbox;
    [SerializeField] private Transform visualRoot;

    [Header("Light Attack")]
    [SerializeField] private float lightDamage = 3f;
    [SerializeField] private float lightAttackDuration = 0.15f;

    [Header("Heavy Attack")]
    [SerializeField] private float heavyDamage = 28f;
    [SerializeField] private float heavyAttackDuration = 0.3f;
    [SerializeField] private float heavyAttackThreshold = 1.5f;

    [Header("Layer")]
    [SerializeField] private LayerMask enemyLayer;

    private float lastAttackTime;
    private bool isAttacking;

    private void Awake()
    {
        // Hitbox starts disabled > only active during an attack
        weaponHitbox.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TriggerAttack();
    }

    private void TriggerAttack()
    {
        if (isAttacking) return;

        float timeSinceLastAttack = Time.time - lastAttackTime;
        bool isHeavy = timeSinceLastAttack >= heavyAttackThreshold;

        if (isHeavy)
            StartCoroutine(PerformAttack(heavyDamage, heavyAttackDuration, true));
        else
            StartCoroutine(PerformAttack(lightDamage, lightAttackDuration, false));

        lastAttackTime = Time.time;
    }

    private System.Collections.IEnumerator PerformAttack(float damage, float duration, bool isHeavy)
    {
        isAttacking = true;
        weaponHitbox.enabled = true;

        UnityEngine.Debug.Log(isHeavy ? $"Heavy Attack! Damage: {damage}" : $"Light Attack! Damage: {damage}");

        // Detect enemies hit during this attack window
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);

        Collider2D[] hits = new Collider2D[10];
        int hitCount = weaponHitbox.Overlap(filter, hits);

        for (int i = 0; i < hitCount; i++)
        {
            Enemy enemy = hits[i].GetComponentInParent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage, isHeavy);
        }

        // Keep hitbox active for the attack duration
        yield return new WaitForSeconds(duration);

        weaponHitbox.enabled = false;
        isAttacking = false;
    }
}

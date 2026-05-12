using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D weaponHitbox;

    [Header("Light Attack")]
    [SerializeField] private float lightDamage = 3f;
    [SerializeField] private float lightAttackDuration = 0.15f;

    [Header("Heavy Attack")]
    [SerializeField] private float heavyDamage = 28f;
    [SerializeField] private float heavyAttackDuration = 0.3f;
    [SerializeField] private float heavyAttackThreshold = 1.5f;

    [Header("Layer")]
    [SerializeField] private LayerMask enemyLayer;

    private float keyHeldTime;
    private bool isAttacking;
    private bool isCharging;

    private Animator _animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        weaponHitbox.enabled = false;
    }

    private void Update()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            keyHeldTime = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isCharging)
            keyHeldTime += Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;
            bool isHeavy = keyHeldTime >= heavyAttackThreshold;

            if (isHeavy)
                StartCoroutine(PerformAttack(heavyDamage, heavyAttackDuration, true));
            else
                StartCoroutine(PerformAttack(lightDamage, lightAttackDuration, false));
        }
    }

    private System.Collections.IEnumerator PerformAttack(float damage, float duration, bool isHeavy)
    {
        isAttacking = true;
        weaponHitbox.enabled = true;

        Debug.Log(isHeavy ? $"Heavy Attack! Damage: {damage}" : $"Light Attack! Damage: {damage}");

        _animator.SetTrigger(AttackHash);

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

        yield return new WaitForSeconds(duration);

        weaponHitbox.enabled = false;
        isAttacking = false;
    }
}
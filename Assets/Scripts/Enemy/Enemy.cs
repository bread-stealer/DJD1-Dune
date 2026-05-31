using UnityEngine;

// Base class for all enemies
public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform visualRoot;

    [Header("Stats")]
    [SerializeField] protected EnemyStats stats;

    protected float currentHealth;
    protected bool isDead;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = visualRoot.GetComponent<Animator>();

        // Stats are optional > Sandworm can leave this unassigned given no combat stats
    }

    protected virtual void Start()
    {
        if (stats != null)
            currentHealth = stats.MaxHealth;
        else
            currentHealth = float.MaxValue;
    }

    protected virtual void Update()
    {
        if (isDead) return;
        EvaluateState();
    }

    protected abstract void EvaluateState();

    public virtual void TakeDamage(float amount, bool isHeavy = false)
    {
        if (isDead) return;

        currentHealth -= amount;
        OnDamaged();

        if (DamageNumberSpawner.Instance != null)
            DamageNumberSpawner.Instance.Spawn(amount, isHeavy, transform.position);

        if (currentHealth <= 0f)
            Die();
    }

    protected virtual void OnDamaged()
    {
    }

    protected virtual void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        OnDeath();
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject, 1f);
    }

    protected bool PlayerInRange(float range)
    {
        if (stats == null) return false;
        return Physics2D.OverlapCircle(transform.position, range, stats.PlayerLayer);
    }

    protected void FacePlayer(Transform player)
    {
        float direction = player.position.x - transform.position.x;
        visualRoot.localScale = new Vector3(Mathf.Sign(direction), 1f, 1f);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.AttackRange);
    }
#endif
}

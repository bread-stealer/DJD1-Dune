using UnityEngine;

// Base class for all enemies
public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform visualRoot;

    protected EnemyStats stats;
    protected float currentHealth;
    protected bool isDead;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Only get Animator if it exists
        animator = visualRoot.GetComponent<Animator>();

        // Ask the child class for its specific stats
        //stats = CreateStats();
        //currentHealth = stats.MaxHealth;
    }

    protected virtual void Update()
    {
        if (isDead) return;
        EvaluateState();
    }

    // Each enemy defines its own stats
    protected abstract EnemyStats CreateStats();
    // Each enemy defines its own behaviour loop
    protected abstract void EvaluateState();

    // Called by weapon/player scripts from outside
    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        OnDamaged();

        if (currentHealth <= 0f)
            Die();
    }

    // Only relevant internally
    // Children can override for hit effects
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
        if (stats == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.AttackRange);
    }
#endif
}

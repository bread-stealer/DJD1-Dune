using UnityEngine;

public class Harkonnen : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float heavyDamageMultiplier = 2f;

    [Header("Patrol Detection")]
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private float ledgeCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stats Override")]
    [SerializeField] private float health = 80f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Detection")]
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private PlayerController _playerController;
    private float lastAttackTime;
    private float patrolDirection = 1f;

    protected override void Awake()
    {
        base.Awake();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            _playerController = playerObj.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("[Harkonnen] No GameObject with tag 'Player' found in scene.");
        }
    }

    protected virtual void Start()
    {
        stats = CreateStats();
        currentHealth = stats.MaxHealth;
    }

    protected override EnemyStats CreateStats()
    {
        return new EnemyStats(
            maxHealth: health,
            moveSpeed: speed,
            damage: attackDamage,
            attackRange: attackRange,
            detectionRange: detectionRange,
            attackCooldown: attackCooldown,
            playerLayer: playerLayer
        );
    }

    protected override void EvaluateState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= stats.AttackRange)
            Attack();
        else if (distanceToPlayer <= stats.DetectionRange)
            Chase();
        else
            Patrol();
    }

    private void Chase()
    {
        FacePlayer(player);

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * stats.MoveSpeed, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", false);
        }
    }

    private void Attack()
    {
        if (_playerController == null) return;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsAttacking", true);
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (Time.time - lastAttackTime < stats.AttackCooldown) return;
        lastAttackTime = Time.time;

        var lightAttack = new AttackData(
            damage: stats.Damage,
            isShieldPenetrating: false
        );

        var heavyAttack = new AttackData(
            damage: stats.Damage * heavyDamageMultiplier,
            isShieldPenetrating: true
        );

        _playerController.TakeDamage(lightAttack);
    }

    private void Patrol()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsWalking", true);
        }

        // Wall check
        RaycastHit2D wallHit = Physics2D.Raycast(
            transform.position,
            Vector2.right * patrolDirection,
            wallCheckDistance,
            groundLayer
        );

        // Ledge check
        Vector2 ledgeCheckOrigin = transform.position + new Vector3(patrolDirection * 0.3f, 0f);
        RaycastHit2D ledgeHit = Physics2D.Raycast(
            ledgeCheckOrigin,
            Vector2.down,
            ledgeCheckDistance,
            groundLayer
        );

        // Flip if wall detected or ledge ahead is gone
        if (wallHit.collider != null || ledgeHit.collider == null)
            patrolDirection *= -1f;

        rb.linearVelocity = new Vector2(patrolDirection * stats.MoveSpeed, rb.linearVelocity.y);
        visualRoot.localScale = new Vector3(patrolDirection, 1f, 1f);
    }

    protected override void OnDamaged()
    {
        if (animator != null)
            animator.SetTrigger("IsHit");
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        // Yellow = detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Red = attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Blue = wall check
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector2.right * patrolDirection * wallCheckDistance);

        // Green = ledge check
        Gizmos.color = Color.green;
        Vector3 ledgeOrigin = transform.position + new Vector3(patrolDirection * 0.3f, 0f);
        Gizmos.DrawRay(ledgeOrigin, Vector2.down * ledgeCheckDistance);
    }
#endif
}
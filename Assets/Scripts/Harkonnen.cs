using UnityEngine;

public class Harkonnen : Enemy
{
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 1.5f;

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
    private float lastAttackTime;
    private int currentPatrolIndex;
    private float patrolWaitTimer;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Start()
    {
        stats = CreateStats();
        currentHealth = stats.MaxHealth;
    }


    protected override EnemyStats CreateStats()
    {
        return new EnemyStats(
            maxHealth:       health,
            moveSpeed:       speed,
            damage:          attackDamage,
            attackRange:     attackRange,
            detectionRange:  detectionRange,
            attackCooldown:  attackCooldown,
            playerLayer:     playerLayer
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

        if (animator != null) animator.SetBool("isWalking", true);
    }

    private void Attack()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);

        if (Time.time - lastAttackTime < stats.AttackCooldown) return;

        lastAttackTime = Time.time;
        if (animator != null) animator.SetTrigger("attack");

        if (PlayerInRange(stats.AttackRange))
            player.GetComponent<PlayerController>().TakeDamage(stats.Damage);
    }

    private void Patrol()
    {
        // No patrol points assigned — just idle
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        // Wait at current patrol point
        patrolWaitTimer -= Time.deltaTime;
        if (patrolWaitTimer > 0f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        // Move towards current patrol point
        Transform target = patrolPoints[currentPatrolIndex];
        float direction = Mathf.Sign(target.position.x - transform.position.x);
        visualRoot.localScale = new Vector3(direction, 1f, 1f);
        rb.linearVelocity = new Vector2(direction * stats.MoveSpeed, rb.linearVelocity.y);
        if (animator != null) animator.SetBool("isWalking", true);

        // Reached patrol point — move to next
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            patrolWaitTimer = patrolWaitTime;
        }
    }

    protected override void OnDamaged()
    {
        // Add hit flash or sound here later
    }

    protected override void OnDeath()
    {
        if (animator != null) animator.SetTrigger("death");
        base.OnDeath();
    }
}

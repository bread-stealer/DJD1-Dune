using UnityEngine;

public class Harkonnen : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float heavyDamageMultiplier = 2f;

    [Header("Patrol Detection")]
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private float ledgeCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Patrol Settings")]
    [SerializeField] private float flipCooldown = 0.3f;

    [Header("Player Reference")]
    [SerializeField] private PlayerController _playerController;

    private Transform player;
    private float lastAttackTime;
    private float lastFlipTime;
    private float patrolDirection = 1f;

    // Cached animator parameter hashes
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int IsHitHash = Animator.StringToHash("IsHit");

    protected override void Awake()
    {
        base.Awake();

        if (_playerController == null)
        {
            Debug.LogError("[Harkonnen] PlayerController reference is not assigned.", this);
            return;
        }

        player = _playerController.transform;
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
            animator.SetBool(IsWalkingHash, true);
            animator.SetBool(IsAttackingHash, false);
        }
    }

    private void Attack()
    {
        if (_playerController == null) return;

        FacePlayer(player);

        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, false);
            animator.SetBool(IsAttackingHash, true);
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
            animator.SetBool(IsAttackingHash, false);
            animator.SetBool(IsWalkingHash, true);
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

        // Flip if wall detected or ledge ahead is gone, with cooldown to prevent rapid flipping
        bool shouldFlip = wallHit.collider != null || ledgeHit.collider == null;
        if (shouldFlip && Time.time - lastFlipTime >= flipCooldown)
        {
            patrolDirection *= -1f;
            lastFlipTime = Time.time;
        }

        rb.linearVelocity = new Vector2(patrolDirection * stats.MoveSpeed, rb.linearVelocity.y);
        visualRoot.localScale = new Vector3(patrolDirection, 1f, 1f);
    }

    protected override void OnDamaged()
    {
        if (animator != null)
            animator.SetTrigger(IsHitHash);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.AttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector2.right * patrolDirection * wallCheckDistance);

        Gizmos.color = Color.green;
        Vector3 ledgeOrigin = transform.position + new Vector3(patrolDirection * 0.3f, 0f);
        Gizmos.DrawRay(ledgeOrigin, Vector2.down * ledgeCheckDistance);
    }
#endif
}
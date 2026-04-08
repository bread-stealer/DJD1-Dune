// Data container for enemy stats
public class EnemyStats
{
    // Read-only from outside
    // Only the constructor can set these
    public float MaxHealth { get; private set; }
    public float MoveSpeed { get; private set; }
    public float Damage { get; private set; }
    public float AttackRange { get; private set; }
    public float DetectionRange { get; private set; }

    // Constructor
    // Called by each enemy child class with its own values
    public EnemyStats(float maxHealth, float moveSpeed, float damage,
                      float attackRange, float detectionRange)
    {
        MaxHealth = maxHealth;
        MoveSpeed = moveSpeed;
        Damage = damage;
        AttackRange = attackRange;
        DetectionRange = detectionRange;
    }
}

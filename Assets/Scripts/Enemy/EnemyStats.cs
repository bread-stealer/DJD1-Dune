using UnityEngine;

// Create via right-click > Create > DUNE > Enemy Stats
[CreateAssetMenu(fileName = "EnemyStats", menuName = "DUNE/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 80f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Combat")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private LayerMask playerLayer;

    // Read-only from outside
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float Damage => damage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float DetectionRange => detectionRange;
    public LayerMask PlayerLayer => playerLayer;
}

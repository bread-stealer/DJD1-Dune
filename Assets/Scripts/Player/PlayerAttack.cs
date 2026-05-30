using System.Collections;
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

    private float _keyHeldTime;
    private bool _isAttacking;
    private bool _isCharging;
    private bool _isHeavy;
    private float _currentDamage;

    private Animator _animator;
    private AnimEventBridge _bridge;
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        weaponHitbox.enabled = false;

        // Subscribe to the animation event bridge on the child sprite
        _bridge = GetComponentInChildren<AnimEventBridge>();
        _bridge.OnAttackHitEvent += OnAttackHit;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        if (_bridge != null)
            _bridge.OnAttackHitEvent -= OnAttackHit;
    }

    private void Update()
    {
        if (_isAttacking) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isCharging = true;
            _keyHeldTime = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && _isCharging)
            _keyHeldTime += Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
        {
            _isCharging = false;
            _isHeavy = _keyHeldTime >= heavyAttackThreshold;
            _currentDamage = _isHeavy ? heavyDamage : lightDamage;

            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;

        // Trigger animation > hit detection fires via Animation Event (OnAttackHit)
        _animator.SetTrigger(AttackHash);

        float duration = _isHeavy ? heavyAttackDuration : lightAttackDuration;
        yield return new WaitForSeconds(duration);

        weaponHitbox.enabled = false;
        _isAttacking = false;
    }

    // Subscribed to AnimationEventBridge.OnAttackHitEvent
    // Fires at the exact frame the knife connects in the animation
    private void OnAttackHit()
    {
        weaponHitbox.enabled = true;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);

        Collider2D[] hits = new Collider2D[10];
        int hitCount = weaponHitbox.Overlap(filter, hits);

        for (int i = 0; i < hitCount; i++)
        {
            Enemy enemy = hits[i].GetComponentInParent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(_currentDamage, _isHeavy);
        }

        Debug.Log(_isHeavy ? $"Heavy Attack! Damage: {_currentDamage}" : $"Light Attack! Damage: {_currentDamage}");
    }
}
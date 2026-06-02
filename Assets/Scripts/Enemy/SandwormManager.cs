using System.Collections;
using UnityEngine;

public class SandwormManager : MonoBehaviour
{
    public static SandwormManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Sandworm sandworm;
    [SerializeField] private PlayerController playerController;

    [Header("Enemies")]
    [SerializeField] private Harkonnen[] harkonnenEnemies;

    [Header("Detection")]
    [SerializeField] private LayerMask sandGroundLayer;
    [SerializeField] private LayerMask safeGroundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;

    [Header("Timing")]
    [SerializeField] private float timeMovingToTrigger = 4f;
    [SerializeField] private float stillDrainRate = 1.5f;
    [SerializeField] private float cooldownAfterEmerge = 8f;

    [Header("Movement Threshold")]
    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Warning Slowdown")]
    [SerializeField] private float slowdownMultiplier = 0.3f;

    private Transform _player;
    private Rigidbody2D _playerRb;
    private float _dangerTimer;
    private float _cooldownTimer;
    private bool _isOnCooldown;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("[SandwormManager] PlayerController reference is not assigned.", this);
            enabled = false;
            return;
        }

        _player = playerController.transform;
        _playerRb = _player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (sandworm == null || _player == null) return;

        if (_isOnCooldown)
        {
            TickCooldown();
            return;
        }

        CheckHarkonnenOnSand();

        if (PlayerOnSafeGround())
        {
            _dangerTimer = 0f;
            return;
        }

        if (!PlayerOnSand())
        {
            _dangerTimer = 0f;
            return;
        }

        if (PlayerIsMoving())
        {
            _dangerTimer += Time.deltaTime;

            if (_dangerTimer >= timeMovingToTrigger)
            {
                TriggerWorm(_player.position);
                _dangerTimer = 0f;
            }
        }
        else
        {
            _dangerTimer = Mathf.Max(0f, _dangerTimer - stillDrainRate * Time.deltaTime);
        }
    }

    private void CheckHarkonnenOnSand()
    {
        if (!sandworm.IsIdle) return;

        foreach (Harkonnen enemy in harkonnenEnemies)
        {
            if (enemy == null) continue;

            if (EnemyOnSand(enemy.transform) && !EnemyOnSafeGround(enemy.transform))
            {
                TriggerWorm(enemy.transform.position);
                return;
            }
        }
    }

    private bool EnemyOnSand(Transform enemy)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            enemy.position + Vector3.up * 0.1f,
            Vector2.down,
            groundCheckDistance + 0.1f,
            sandGroundLayer
        );
        return hit.collider != null;
    }

    private bool EnemyOnSafeGround(Transform enemy)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            enemy.position + Vector3.up * 0.1f,
            Vector2.down,
            groundCheckDistance + 0.1f,
            safeGroundLayer
        );
        return hit.collider != null;
    }

    private bool PlayerIsMoving()
    {
        if (_playerRb == null) return false;
        return Mathf.Abs(_playerRb.linearVelocity.x) > movementThreshold;
    }

    private void TriggerWorm(Vector3 position)
    {
        if (!sandworm.IsIdle) return;
        sandworm.Trigger(position);
        _isOnCooldown = true;
        _cooldownTimer = cooldownAfterEmerge;
        StartCoroutine(SlowdownRoutine());
    }

    private IEnumerator SlowdownRoutine()
    {
        // Apply slowdown during warning phase
        playerController.SetSpeedMultiplier(slowdownMultiplier);
        foreach (Harkonnen enemy in harkonnenEnemies)
        {
            if (enemy != null)
                enemy.SetSpeedMultiplier(slowdownMultiplier);
        }

        yield return new WaitForSeconds(sandworm.WarningDuration);

        // Restore speed after warning ends
        playerController.SetSpeedMultiplier(1f);
        foreach (Harkonnen enemy in harkonnenEnemies)
        {
            if (enemy != null)
                enemy.SetSpeedMultiplier(1f);
        }
    }

    private bool PlayerOnSand()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            _player.position + Vector3.up * 0.1f,
            Vector2.down,
            groundCheckDistance + 0.1f,
            sandGroundLayer
        );
        return hit.collider != null;
    }

    private bool PlayerOnSafeGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            _player.position + Vector3.up * 0.1f,
            Vector2.down,
            groundCheckDistance + 0.1f,
            safeGroundLayer
        );
        return hit.collider != null;
    }

    private void TickCooldown()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
            _isOnCooldown = false;
    }
}

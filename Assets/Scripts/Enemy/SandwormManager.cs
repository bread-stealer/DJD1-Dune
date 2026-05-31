using UnityEngine;

public class SandwormManager : MonoBehaviour
{
    public static SandwormManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Sandworm sandworm;
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private LayerMask sandGroundLayer;
    [SerializeField] private LayerMask safeGroundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;

    [Header("Timing")]
    [SerializeField] private float timeOnSandToTrigger = 5f;
    [SerializeField] private float cooldownAfterEmerge = 8f;

    private float _timeOnSand;
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

    private void Update()
    {
        if (sandworm == null || player == null) return;
        if (_isOnCooldown)
        {
            TickCooldown();
            return;
        }

        if (PlayerOnSand() && !PlayerOnSafeGround())
        {
            _timeOnSand += Time.deltaTime;

            if (_timeOnSand >= timeOnSandToTrigger)
            {
                TriggerWorm();
                _timeOnSand = 0f;
            }
        }
        else
        {
            _timeOnSand = 0f;
        }
    }

    private void TriggerWorm()
    {
        sandworm.Trigger(player.position);
        _isOnCooldown = true;
        _cooldownTimer = cooldownAfterEmerge;
    }

    private void TickCooldown()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
            _isOnCooldown = false;
    }

    private bool PlayerOnSand()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            player.position + Vector3.up * 0.1f,
            Vector2.down,
            groundCheckDistance + 0.1f,
            sandGroundLayer
        );
        return hit.collider != null;
    }

    private bool PlayerOnSafeGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            player.position + Vector3.up * 0.1f,
            Vector2.down,
            groundCheckDistance + 0.1f,
            safeGroundLayer
        );
        return hit.collider != null;
    }
}

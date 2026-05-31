using System.Collections;
using UnityEngine;

public class Sandworm : Enemy
{
    [Header("Trigger")]
    [SerializeField] private float triggerRadius = 8f;
    [SerializeField] private float emergeDelay = 5f;
    [SerializeField] private float escapeRadius = 12f;

    [Header("Attack")]
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private float burstDuration = 1f;
    [SerializeField] private float disappearDelay = 3f;
    // How high the worm rises above the ground
    [SerializeField] private float burstHeight = 3f;

    [Header("References")]
    [SerializeField] private SpriteRenderer wormSprite;
    [SerializeField] private ParticleSystem sandParticles;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private WormHead wormHead;

    // Assign rocky platform layers here > worm won't trigger if player is standing on these
    [Header("Safe Ground")]
    [SerializeField] private LayerMask safeGroundLayer;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Header("Stats")]
    [SerializeField] private LayerMask playerLayer;

    private enum WormState { Waiting, Countdown, Warning, Emerging, Underground }
    private WormState _state = WormState.Waiting;

    [Header("Player Reference")]
    [SerializeField] private Transform _player;

    private float _countdownTimer;
    private Vector3 _groundPosition;
    private Vector3 _originalSpriteScale;

    protected override EnemyStats CreateStats()
    {
        // Sandworm has no combat stats > instakill is handled by WormHead separately
        return new EnemyStats(
            maxHealth: 1f,
            moveSpeed: 0f,
            damage: 0f,
            attackRange: 0f,
            detectionRange: triggerRadius,
            attackCooldown: 0f,
            playerLayer: playerLayer
        );
    }

    protected override void Awake()
    {
        base.Awake();

        // Cache original sprite scale to preserve it when flipping
        _originalSpriteScale = wormSprite.transform.localScale;

        // Worm starts hidden underground
        if (wormSprite != null)
            wormSprite.enabled = false;

        // WormHead kill zone starts inactive
        if (wormHead != null)
            wormHead.SetActive(false);
    }

    private void Start()
    {
        if (_player == null)
            Debug.LogError("[Sandworm] Player Transform reference is not assigned.", this);

        // Cache the ground position so the worm always returns here after bursting
        _groundPosition = transform.position;
    }

    protected override void EvaluateState()
    {
        if (_player == null) return;

        switch (_state)
        {
            case WormState.Waiting:
                CheckPlayerInRange();
                break;

            case WormState.Countdown:
                TickCountdown();
                break;
        }
    }

    private bool PlayerOnSafeGround()
    {
        // Raycast downward from the player to check if they're standing on a safe layer
        RaycastHit2D hit = Physics2D.Raycast(
            _player.position,
            Vector2.down,
            groundCheckDistance,
            safeGroundLayer
        );
        return hit.collider != null;
    }

    private void CheckPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, _player.position);

        // Only trigger if player is close and not standing on safe ground
        if (distance <= triggerRadius && !PlayerOnSafeGround())
        {
            _countdownTimer = emergeDelay;
            _state = WormState.Countdown;
            Debug.Log("[Sandworm] Player entered area > countdown started.");
        }
    }

    private void TickCountdown()
    {
        float distance = Vector2.Distance(transform.position, _player.position);

        // If player escapes or reaches safe ground, reset
        if (distance > escapeRadius || PlayerOnSafeGround())
        {
            _state = WormState.Waiting;
            Debug.Log("[Sandworm] Player escaped, resetting.");
            return;
        }

        _countdownTimer -= Time.deltaTime;
        if (_countdownTimer <= 0f)
            StartCoroutine(EmergeSequence());
    }

    private IEnumerator EmergeSequence()
    {
        _state = WormState.Warning;

        // Snap worm to player's current X position along the ground
        transform.position = new Vector3(_player.position.x, _groundPosition.y, _groundPosition.z);

        // Warning phase > camera shake before burst
        if (cameraShake != null)
            cameraShake.StartShake(warningDuration, 0.3f);

        if (sandParticles != null)
            sandParticles.Play();

        yield return new WaitForSeconds(warningDuration);

        // Emerge phase > worm bursts up from ground
        _state = WormState.Emerging;
        wormSprite.transform.localScale = _originalSpriteScale;
        wormSprite.enabled = true;

        // Activate WormHead kill zone for the duration of the burst
        if (wormHead != null)
            wormHead.SetActive(true);

        // OnTriggerEnter2D on WormHead handles the kill while state is Emerging
        yield return StartCoroutine(BurstAnimation());

        yield return new WaitForSeconds(burstDuration);

        // Deactivate kill zone before burrowing back
        if (wormHead != null)
            wormHead.SetActive(false);

        // Burrow back underground
        _state = WormState.Underground;
        wormSprite.enabled = false;

        if (sandParticles != null)
            sandParticles.Stop();

        // Wait then reset so worm can trigger again
        yield return new WaitForSeconds(disappearDelay);
        _state = WormState.Waiting;
        Debug.Log("[Sandworm] Worm reset, waiting.");
    }

    private IEnumerator BurstAnimation()
    {
        Vector3 groundPosition = transform.position;
        Vector3 peakPosition = groundPosition + Vector3.up * burstHeight;

        float elapsed = 0f;
        float halfDuration = burstDuration * 0.5f;

        // Rise up > fast at start, slows at peak
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 2f); // Ease out
            transform.position = Vector3.Lerp(groundPosition, peakPosition, easedT);
            yield return null;
        }

        elapsed = 0f;

        // Fall back down > slow at start, fast at bottom
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float easedT = Mathf.Pow(t, 2f);
            transform.position = Vector3.Lerp(peakPosition, groundPosition, easedT);
            yield return null;
        }

        // Flip sprite to show worm diving back underground
        wormSprite.transform.localScale = new Vector3(_originalSpriteScale.x, -_originalSpriteScale.y, _originalSpriteScale.z);
        transform.position = groundPosition;
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        // Yellow = trigger radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        // Red = escape radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, escapeRadius);
    }
#endif
}